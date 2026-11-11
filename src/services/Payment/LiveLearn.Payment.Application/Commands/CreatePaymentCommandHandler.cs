using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Application.Configuration;
using LiveLearn.Payment.Application.Repositories;
using LiveLearn.Payment.Domain.Entities;
using Microsoft.Extensions.Options;

namespace LiveLearn.Payment.Application.Commands;


internal sealed class CreatePaymentCommandHandler(
    IOrderPaymentRepository orderPaymentRepository,
    IUnitOfWork unitOfWork,
    IOptions<RevenueShareOptions> revenueShareOptions,
    TimeProvider timeProvider) : ICommandHandler<CreatePaymentCommand>
{
    public async Task<Result> Handle(CreatePaymentCommand request, CancellationToken ct)
    {
        // Idempotent per OrderId (D10): a redelivered or resent request finds the payment and changes
        // nothing. A concurrent duplicate fails on the unique OrderId index and its retry lands here.
        if (await orderPaymentRepository.GetByOrderIdAsync(request.OrderId, ct) is not null)
            return Result.Success();

        var payment = OrderPayment.Create(
            request.OrderId,
            request.StudentId,
            request.Lines,
            revenueShareOptions.Value.TutorShareRate,
            timeProvider.GetUtcNow());

        if (!payment.IsSuccess)
            return Result.Failure(payment.Errors);

        await orderPaymentRepository.AddAsync(payment.Value, ct);
        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
