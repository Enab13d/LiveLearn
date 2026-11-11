using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Application.Dto;
using LiveLearn.Payment.Application.Queries;
using LiveLearn.Payment.Application.Services;
using LiveLearn.Payment.Domain.Enums;
using LiveLearn.Payment.Domain.Errors;
using LiveLearn.Payment.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Payment.Infrastructure.Queries.Handlers;


internal sealed class GetCheckoutQueryHandler(ICurrentUser currentUser, ReadDbContext dbContext) : IQueryHandler<GetCheckoutQuery, CheckoutDto>
{
    public async Task<Result<CheckoutDto>> Handle(GetCheckoutQuery request, CancellationToken ct)
    {
        var payment = await dbContext.OrderPayments
            .Where(e => e.OrderId == request.OrderId && e.StudentId == currentUser.UserId)
            .Select(e => new { e.Status, e.CheckoutUrl })
            .FirstOrDefaultAsync(ct);

        if (payment is null) return Result<CheckoutDto>.Failure(OrderPaymentErrors.NotFound);

        // Paid, expired or rejected: the URL is useless now, even if it exists.
        if (payment.Status != nameof(OrderPaymentStatus.AwaitingPayment))
            return Result<CheckoutDto>.Failure(OrderPaymentErrors.NotAwaitingPayment);

        return new CheckoutDto(payment.CheckoutUrl);
    }
}
