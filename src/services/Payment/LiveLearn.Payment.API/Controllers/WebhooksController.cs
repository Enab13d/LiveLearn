using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Application.Commands;
using LiveLearn.Payment.Infrastructure.Configuration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace LiveLearn.Payment.API.Controllers;


// Called by Stripe, not by users: the signature replaces the JWT. The matching command runs inline;
// a failure returns 5xx and Stripe redelivers, so every command must be idempotent (D15).
[Authorize]
[ApiController]
[Route("webhooks")]
public sealed class WebhooksController(
    ISender mediator,
    IOptions<StripeOptions> stripeOptions,
    ILogger<WebhooksController> logger) : ControllerBase
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

        Result result;
        switch (stripeEvent.Data.Object)
        {
            case Session session when stripeEvent.Type == EventTypes.CheckoutSessionCompleted:
                result = await mediator.Send(new ConfirmPaymentCommand(session.Id), ct);
                break;

            case Session session when stripeEvent.Type == EventTypes.CheckoutSessionExpired:
                result = await mediator.Send(new ExpirePaymentCommand(session.Id), ct);
                break;

            // Disputes are out of scope: logged for a person to handle (D7).
            case Dispute dispute:
                logger.LogWarning(
                    "ALERT: Stripe dispute event {EventType} {EventId} for dispute {DisputeId}",
                    stripeEvent.Type, stripeEvent.Id, dispute.Id);
                return Ok();

            default:
                logger.LogInformation("Ignored Stripe event {EventType} {EventId}", stripeEvent.Type, stripeEvent.Id);
                return Ok();
        }

        // 5xx, not the error's own status: any failure here is worth a redelivery (D15).
        if (!result.IsSuccess)
        {
            logger.LogError("Stripe event {EventType} {EventId} failed: {@Errors}", stripeEvent.Type, stripeEvent.Id, result.Errors);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok();
    }
}
