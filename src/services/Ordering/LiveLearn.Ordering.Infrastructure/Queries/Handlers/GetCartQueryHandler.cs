using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Application.Dto;
using LiveLearn.Ordering.Application.Queries;
using LiveLearn.Ordering.Application.Services;
using LiveLearn.Ordering.Domain.Common;

namespace LiveLearn.Ordering.Infrastructure.Queries.Handlers;


internal sealed class GetCartQueryHandler(
    ICurrentUser currentUser,
    ICartStore cartStore,
    PurchaseFactsReader purchaseFactsReader) : IQueryHandler<GetCartQuery, CartDto>
{
    public async Task<Result<CartDto>> Handle(GetCartQuery request, CancellationToken ct)
    {
        var studentId = currentUser.UserId;

        var cart = await cartStore.GetAsync(studentId, ct);
        var facts = await purchaseFactsReader.ReadAsync(studentId, cart.Select(i => i.CourseId).ToList(), ct);

        var items = cart.Select(item =>
        {
            var itemFacts = facts[item.CourseId];
            var availability = PurchaseRules.Evaluate(itemFacts, studentId);
            var currentPrice = itemFacts.Course?.Price;

            return new CartItemDto(
                item.CourseId,
                item.AddedAt,
                item.PriceAtAdd,
                currentPrice,
                currentPrice is not null && currentPrice != item.PriceAtAdd,
                availability.IsAvailable,
                availability.Reason?.ToString(),
                availability.PendingOrderId);
        }).ToList();

        var total = items.Where(i => i.IsAvailable).Sum(i => i.CurrentPrice!.Value);

        return new CartDto(items, total);
    }
}
