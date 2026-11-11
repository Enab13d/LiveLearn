using System.Net;
using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Application.Errors;
using LiveLearn.Payment.Application.Services;
using Stripe;

namespace LiveLearn.Payment.Infrastructure.Services;


internal sealed class StripeRefundGateway(StripeClient stripeClient) : IRefundGateway
{
    private const string OrderIdKey = "OrderId";
    private const string OrderLineIdKey = "OrderLineId";

    private readonly RefundService _refunds = new(stripeClient);

    public async Task<Result<string>> CreateRefundAsync(RefundRequest request, string idempotencyKey, CancellationToken ct)
    {
        var createOptions = new RefundCreateOptions
        {
            Charge = request.ChargeId,
            // A partial refund of the order's charge: the line price (D6).
            Amount = StripeAmounts.ToMinorUnits(request.Amount),
            Metadata = new Dictionary<string, string>
            {
                [OrderIdKey] = request.OrderId.ToString(),
                [OrderLineIdKey] = request.OrderLineId.ToString(),
            },
        };

        try
        {
            var refund = await _refunds.CreateAsync(createOptions, new RequestOptions { IdempotencyKey = idempotencyKey }, ct);
            return refund.Id;
        }
        // 400 / 402: Stripe refused the request itself (e.g. the charge is already refunded), and resending
        // it changes nothing. Anything else (network, 429, 5xx) propagates so the consumer retries (D14).
        catch (StripeException ex) when (ex.HttpStatusCode is HttpStatusCode.BadRequest or HttpStatusCode.PaymentRequired)
        {
            return Result<string>.Failure(RefundErrors.Rejected(ex.StripeError?.Message ?? ex.Message));
        }
    }

    public async Task<Result<RefundState>> GetRefundAsync(string refundId, CancellationToken ct)
    {
        Refund refund;
        try
        {
            refund = await _refunds.GetAsync(refundId, cancellationToken: ct);
        }
        catch (StripeException ex) when (ex.HttpStatusCode is HttpStatusCode.NotFound)
        {
            return Result<RefundState>.Failure(RefundErrors.UnknownRefund(refundId));
        }

        // Refunds not created by this service (e.g. from the Dashboard) carry no metadata.
        if (!Guid.TryParse(refund.Metadata?.GetValueOrDefault(OrderIdKey), out var orderId)
            || !Guid.TryParse(refund.Metadata?.GetValueOrDefault(OrderLineIdKey), out var orderLineId))
            return Result<RefundState>.Failure(RefundErrors.UnknownRefund(refundId));

        var status = refund.Status switch
        {
            "succeeded" => RefundStatus.Succeeded,
            "failed" => RefundStatus.Failed,
            "canceled" => RefundStatus.Canceled,
            // "pending", "requires_action": not settled yet, a later webhook reports the outcome.
            _ => RefundStatus.Pending,
        };

        return new RefundState(orderId, orderLineId, status);
    }
}
