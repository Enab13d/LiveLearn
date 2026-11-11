using System.Net;
using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Application.Errors;
using LiveLearn.Payment.Application.Services;
using LiveLearn.Payment.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace LiveLearn.Payment.Infrastructure.Services;


internal sealed class StripeCheckoutGateway(StripeClient stripeClient, IOptions<StripeOptions> options) : ICheckoutGateway
{
    private const string OrderIdKey = "OrderId";

    private readonly SessionService _sessions = new(stripeClient);

    public async Task<Result<CheckoutSession>> CreateSessionAsync(CheckoutRequest request, string idempotencyKey, CancellationToken ct)
    {
        var orderId = request.OrderId.ToString();

        var createOptions = new SessionCreateOptions
        {
            Mode = "payment",
            ClientReferenceId = orderId,
            Metadata = new Dictionary<string, string> { [OrderIdKey] = orderId },
            ExpiresAt = request.ExpiresAt.UtcDateTime,
            SuccessUrl = options.Value.CheckoutSuccessUrl,
            CancelUrl = options.Value.CheckoutCancelUrl,
            // "card" includes Apple Pay and Google Pay; no delayed methods, so no async_payment events (D1).
            AllowedPaymentMethodTypes = ["card"],
            // USD only: the student pays the listed USD price (D1).
            AdaptivePricing = new SessionAdaptivePricingOptions { Enabled = false },
            LineItems = request.Items.Select(item => new SessionLineItemOptions
            {
                Quantity = 1,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = item.Price.Currency.ToLowerInvariant(),
                    UnitAmount = StripeAmounts.ToMinorUnits(item.Price),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.CourseTitle,
                        Metadata = new Dictionary<string, string> { ["CourseId"] = item.CourseId.ToString() },
                    },
                },
            }).ToList(),
        };

        try
        {
            var session = await _sessions.CreateAsync(createOptions, new RequestOptions { IdempotencyKey = idempotencyKey }, ct);
            return new CheckoutSession(session.Id, session.Url);
        }
        // 400 / 402: Stripe refused the request itself, and resending it changes nothing. Anything else
        // (network, 429, 5xx) propagates so the consumer retries (D14).
        catch (StripeException ex) when (ex.HttpStatusCode is HttpStatusCode.BadRequest or HttpStatusCode.PaymentRequired)
        {
            return Result<CheckoutSession>.Failure(CheckoutErrors.Rejected(ex.StripeError?.Message ?? ex.Message));
        }
    }

    public async Task<Result<CompletedCheckoutSession>> GetCompletedSessionAsync(string sessionId, CancellationToken ct)
    {
        var getOptions = new SessionGetOptions
        {
            // The fee is read from the charge's balance transaction (D14).
            Expand = ["payment_intent.latest_charge.balance_transaction"],
        };

        Session session;
        try
        {
            session = await _sessions.GetAsync(sessionId, getOptions, cancellationToken: ct);
        }
        catch (StripeException ex) when (ex.HttpStatusCode is HttpStatusCode.NotFound)
        {
            return Result<CompletedCheckoutSession>.Failure(CheckoutErrors.UnknownSession(sessionId));
        }

        // Sessions not created by this service (e.g. `stripe trigger`) carry no OrderId.
        var orderIdValue = session.Metadata?.GetValueOrDefault(OrderIdKey) ?? session.ClientReferenceId;
        if (!Guid.TryParse(orderIdValue, out var orderId))
            return Result<CompletedCheckoutSession>.Failure(CheckoutErrors.UnknownSession(sessionId));

        if (session.PaymentStatus != "paid")
            return new CompletedCheckoutSession(orderId, IsPaid: false, ChargeId: null, Fee: null, PaidAt: null);

        var charge = session.PaymentIntent?.LatestCharge
            ?? throw new InvalidOperationException($"Paid checkout session '{sessionId}' has no charge.");
        var balanceTransaction = charge.BalanceTransaction
            ?? throw new InvalidOperationException($"Charge '{charge.Id}' has no balance transaction yet.");

        var fee = StripeAmounts.FromMinorUnits(balanceTransaction.Fee, balanceTransaction.Currency);

        return new CompletedCheckoutSession(orderId, IsPaid: true, charge.Id, fee, new DateTimeOffset(charge.Created, TimeSpan.Zero));
    }
}
