using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Application.Configuration;
using LiveLearn.Payment.Application.Repositories;
using LiveLearn.Payment.Application.Services;
using LiveLearn.Payment.Domain.Errors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LiveLearn.Payment.Application.Commands;


internal sealed class ConfirmPaymentCommandHandler(
    ICheckoutGateway checkoutGateway,
    IOrderPaymentRepository orderPaymentRepository,
    IUnitOfWork unitOfWork,
    IOptions<RefundOptions> refundOptions,
    ILogger<ConfirmPaymentCommandHandler> logger) : ICommandHandler<ConfirmPaymentCommand>
{
    public async Task<Result> Handle(ConfirmPaymentCommand request, CancellationToken ct)
    {
        // Re-read instead of trusting the event body: the fee comes from the expanded balance transaction (D14).
        var session = await checkoutGateway.GetCompletedSessionAsync(request.SessionId, ct);
        if (!session.IsSuccess)
            return Result.Failure(session.Errors);

        // Cards only, so a completed session is paid; an unpaid one has nothing to confirm.
        if (!session.Value.IsPaid)
        {
            logger.LogWarning("Checkout session {SessionId} for order {OrderId} completed unpaid", request.SessionId, session.Value.OrderId);
            return Result.Success();
        }

        var payment = await orderPaymentRepository.GetByOrderIdAsync(session.Value.OrderId, ct);
        if (payment is null)
            return Result.Failure(OrderPaymentErrors.NotFound);

        // ChargeId, Fee and PaidAt are always set on a paid session. A redelivery is a no-op in MarkPaid.
        payment.MarkPaid(
            session.Value.ChargeId!,
            session.Value.Fee!,
            session.Value.PaidAt!.Value,
            TimeSpan.FromDays(refundOptions.Value.WindowDays));

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
