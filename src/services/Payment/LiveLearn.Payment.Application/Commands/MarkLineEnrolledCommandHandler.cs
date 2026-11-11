using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Application.Repositories;
using LiveLearn.Payment.Domain.Errors;

namespace LiveLearn.Payment.Application.Commands;


internal sealed class MarkLineEnrolledCommandHandler(
    IOrderPaymentRepository orderPaymentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<MarkLineEnrolledCommand>
{
    public async Task<Result> Handle(MarkLineEnrolledCommand request, CancellationToken ct)
    {
        var payment = await orderPaymentRepository.GetByOrderIdAsync(request.OrderId, ct);
        if (payment is null)
            return Result.Failure(OrderPaymentErrors.NotFound);

        // A redelivery is a no-op in MarkLineEnrolled.
        var result = payment.MarkLineEnrolled(request.OrderLineId);
        if (!result.IsSuccess)
            return result;

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
