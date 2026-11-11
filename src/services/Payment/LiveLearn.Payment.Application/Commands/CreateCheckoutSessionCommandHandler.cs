using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Application.Repositories;
using LiveLearn.Payment.Application.Services;
using LiveLearn.Payment.Domain.Enums;
using LiveLearn.Payment.Domain.Errors;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Payment.Application.Commands;


internal sealed class CreateCheckoutSessionCommandHandler(
    IOrderPaymentRepository orderPaymentRepository,
    ICheckoutGateway checkoutGateway,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ILogger<CreateCheckoutSessionCommandHandler> logger) : ICommandHandler<CreateCheckoutSessionCommand>
{
    // Counted from the payment's CreatedAt, so every retry sends the same expiry (D14).
    private static readonly TimeSpan SessionLifetime = TimeSpan.FromHours(1);

    public async Task<Result> Handle(CreateCheckoutSessionCommand request, CancellationToken ct)
    {
        var payment = await orderPaymentRepository.GetByOrderIdAsync(request.OrderId, ct);
        if (payment is null)
            return Result.Failure(OrderPaymentErrors.NotFound);

        // Redelivery: the session is attached or the payment has already moved on.
        if (payment.Status != OrderPaymentStatus.AwaitingPayment || payment.CheckoutSessionId is not null)
            return Result.Success();

        var checkoutRequest = new CheckoutRequest(
            payment.OrderId,
            payment.CreatedAt + SessionLifetime,
            payment.Lines.Select(l => new CheckoutLineItem(l.CourseId, l.CourseTitle, l.Price)).ToList());

        // Idempotency key OrderId: a retry after a lost response gets the same session back (D14).
        var session = await checkoutGateway.CreateSessionAsync(checkoutRequest, payment.OrderId.ToString(), ct);

        if (session.IsSuccess)
        {
            payment.AttachCheckoutSession(session.Value.SessionId, session.Value.Url);
        }
        else
        {
            // A permanent rejection is an outcome, not a fault: the payment ends here and its domain event
            // replies PaymentRequestRejected to the saga.
            logger.LogWarning("Checkout session for order {OrderId} rejected: {@Errors}", payment.OrderId, session.Errors);
            payment.Reject(session.FirstError.Message, timeProvider.GetUtcNow());
        }

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
