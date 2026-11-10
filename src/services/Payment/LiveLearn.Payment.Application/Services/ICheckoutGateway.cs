using LiveLearn.BuildingBlocks;

namespace LiveLearn.Payment.Application.Services;


// Hosted checkout at the PSP (D1, D9). No PSP types cross this port.
public interface ICheckoutGateway
{
    // Creates the session once per idempotency key; a retry with the same request returns the same session.
    // A permanent rejection (the PSP refused the request itself) is a failure; transient errors throw so
    // the caller's retry runs again (D14).
    Task<Result<CheckoutSession>> CreateSessionAsync(CheckoutRequest request, string idempotencyKey, CancellationToken ct);

    // Re-reads the session for a webhook (D14, D15). A session that isn't ours (no OrderId) or doesn't
    // exist is a failure.
    Task<Result<CompletedCheckoutSession>> GetCompletedSessionAsync(string sessionId, CancellationToken ct);
}
