using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Application.Repositories;
using LiveLearn.Payment.Application.Services;
using LiveLearn.Payment.Domain.Errors;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Payment.Application.Commands;


internal sealed class RequestLineRefundCommandHandler(
    IOrderPaymentRepository orderPaymentRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    TimeProvider timeProvider,
    ILogger<RequestLineRefundCommandHandler> logger) : ICommandHandler<RequestLineRefundCommand>
{
    public async Task<Result> Handle(RequestLineRefundCommand request, CancellationToken ct)
    {
        var payment = await orderPaymentRepository.GetByOrderIdAsync(request.OrderId, ct);

        // A student's request for someone else's order looks like a missing one. The saga has no
        // current user, so it is checked only for the student.
        if (payment is null || (request.ByStudent && payment.StudentId != currentUser.UserId))
            return Result.Failure(OrderPaymentErrors.NotFound);

        // The domain event handler stages IssueLineRefund; Stripe is called only after this commit (D14).
        var result = payment.RequestLineRefund(request.OrderLineId, request.ByStudent, timeProvider.GetUtcNow());
        if (!result.IsSuccess)
            return result;

        await unitOfWork.CommitAsync(ct);

        // The reason is kept for support only; nothing downstream depends on it.
        logger.LogInformation(
            "Refund requested for order {OrderId} line {OrderLineId} (by student: {ByStudent}): {Reason}",
            request.OrderId, request.OrderLineId, request.ByStudent, request.Reason);

        return Result.Success();
    }
}
