using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Application.Repositories;
using LiveLearn.Ordering.Domain.Enums;
using LiveLearn.Ordering.Domain.Errors;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Ordering.Application.Commands;


internal sealed class RecordLineRefundFailedCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    ILogger<RecordLineRefundFailedCommandHandler> logger) : ICommandHandler<RecordLineRefundFailedCommand>
{
    public async Task<Result> Handle(RecordLineRefundFailedCommand request, CancellationToken ct)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, ct);
        if (order is null) return Result.Failure(OrderErrors.NotFound);

        var result = order.MarkLineRefundFailed(request.OrderLineId);
        if (!result.IsSuccess) return Result.Failure(result.Errors);

        await unitOfWork.CommitAsync(ct);

        // Alert: access stays revoked and the student is still owed the money, so support returns it by other means.
        if (order.Lines.Single(e => e.Id == request.OrderLineId).Status == OrderLineStatus.RefundFailed)
            logger.LogError("ALERT: refund of order line {OrderLineId} in order {OrderId} failed; the student {StudentId} must be refunded manually",
                request.OrderLineId, request.OrderId, order.StudentId);

        return Result.Success();
    }
}
