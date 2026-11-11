using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Application.Repositories;
using LiveLearn.Payment.Application.Services;
using LiveLearn.Payment.Domain.Errors;

namespace LiveLearn.Payment.Application.Commands;


internal sealed class ExpirePaymentCommandHandler(
    ICheckoutGateway checkoutGateway,
    IOrderPaymentRepository orderPaymentRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<ExpirePaymentCommand>
{
    public async Task<Result> Handle(ExpirePaymentCommand request, CancellationToken ct)
    {
        // The webhook carries no trace context; OrderId comes from the session's metadata (C1).
        var session = await checkoutGateway.GetCompletedSessionAsync(request.SessionId, ct);
        if (!session.IsSuccess)
            return Result.Failure(session.Errors);

        var payment = await orderPaymentRepository.GetByOrderIdAsync(session.Value.OrderId, ct);
        if (payment is null)
            return Result.Failure(OrderPaymentErrors.NotFound);

        // A redelivery, or an expiry after the payment moved on, is a no-op in Expire.
        payment.Expire(timeProvider.GetUtcNow());

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
