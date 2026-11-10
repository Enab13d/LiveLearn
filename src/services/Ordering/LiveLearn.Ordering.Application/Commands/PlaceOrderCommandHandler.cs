using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Application.Repositories;
using LiveLearn.Ordering.Application.Services;
using LiveLearn.Ordering.Domain.Common;
using LiveLearn.Ordering.Domain.Entities;
using LiveLearn.Ordering.Domain.Errors;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Ordering.Application.Commands;


internal sealed class PlaceOrderCommandHandler(
    ICurrentUser currentUser,
    ICartStore cartStore,
    PurchaseFactsReader purchaseFactsReader,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ILogger<PlaceOrderCommandHandler> logger) : ICommandHandler<PlaceOrderCommand, Guid>
{
    public async Task<Result<Guid>> Handle(PlaceOrderCommand request, CancellationToken ct)
    {
        var studentId = currentUser.UserId;

        var cart = await cartStore.GetAsync(studentId, ct);
        var facts = await purchaseFactsReader.ReadAsync(studentId, cart.Select(i => i.CourseId).ToList(), ct);

        // Snapshot owner and price from the projection now — never priceAtAdd (O3, O5).
        var lines = cart
            .Select(i => facts[i.CourseId])
            .Where(f => PurchaseRules.Evaluate(f, studentId).IsAvailable)
            .Select(f => new OrderLineSnapshot(f.Course!.CourseId, f.Course.OwnerId!.Value, f.Course.Price!.Value))
            .ToList();

        if (lines.Count == 0) return Result<Guid>.Failure(OrderErrors.NothingToOrder);
        if (lines.Sum(l => l.Price) != request.ExpectedTotal) return Result<Guid>.Failure(OrderErrors.TotalMismatch);

        // Order.Create raises OrderPlacedDomainEvent; CommitAsync dispatches it and its handler stages OrderPlaced.
        var order = Order.Create(Guid.NewGuid(), studentId, timeProvider.GetUtcNow(), lines);

        await orderRepository.AddAsync(order, ct);

        // A parallel placement that loses the race on the active-line unique index throws here → 500 (accepted, O6).
        await unitOfWork.CommitAsync(ct);

        // Best effort: the order is committed; leftovers show up in the cart as PendingOrder (O5).
        try
        {
            await cartStore.RemoveAsync(studentId, lines.Select(l => l.CourseId).ToList(), ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Could not remove the items of order {OrderId} from the cart of student {StudentId}", order.Id, studentId);
        }

        return order.Id;
    }
}
