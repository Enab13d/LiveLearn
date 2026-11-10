using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Application.Repositories;
using LiveLearn.Ordering.Domain.Errors;

namespace LiveLearn.Ordering.Application.Commands;


internal sealed class RecordLineRefundedCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RecordLineRefundedCommand>
{
    public async Task<Result> Handle(RecordLineRefundedCommand request, CancellationToken ct)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, ct);
        if (order is null) return Result.Failure(OrderErrors.NotFound);

        // A redelivered refund is a no-op in the aggregate.
        var result = order.MarkLineRefunded(request.OrderLineId);
        if (!result.IsSuccess) return Result.Failure(result.Errors);

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
