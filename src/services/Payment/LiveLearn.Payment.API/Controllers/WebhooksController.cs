using LiveLearn.Payment.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace LiveLearn.Payment.API.Controllers;


// Called by Stripe, not by users: the signature replaces the JWT. The matching command runs inline;
// a failure returns 5xx and Stripe redelivers, so every command must be idempotent (D15).
[Authorize]
[ApiController]
[Route("webhooks")]
public sealed class WebhooksController(IOptions<StripeOptions> stripeOptions, ILogger<WebhooksController> logger)
    : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("stripe")]
    public async Task<IActionResult> Stripe(CancellationToken ct)
    {
        // The signature covers the exact bytes, so the body is read raw.
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync(ct);

        Event stripeEvent;
        try
        {
            // Also rejects events rendered in an API version other than the one Stripe.net pins.
            stripeEvent = EventUtility.ConstructEvent(
                payload, Request.Headers["Stripe-Signature"], stripeOptions.Value.PlatformWebhookSecret);
        }
        catch (StripeException ex)
        {
            logger.LogWarning(ex, "Rejected a Stripe webhook");
            return BadRequest();
        }

        switch (stripeEvent.Data.Object)
        {
            // Disputes are out of scope: logged for a person to handle (D7).
            case Dispute dispute:
                logger.LogWarning(
                    "ALERT: Stripe dispute event {EventType} {EventId} for dispute {DisputeId}",
                    stripeEvent.Type, stripeEvent.Id, dispute.Id);
                break;

            default:
                logger.LogInformation("Ignored Stripe event {EventType} {EventId}", stripeEvent.Type, stripeEvent.Id);
                break;
        }

        return Ok();
    }
}
