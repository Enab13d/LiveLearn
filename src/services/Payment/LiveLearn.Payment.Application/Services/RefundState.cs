namespace LiveLearn.Payment.Application.Services;


// OrderId and OrderLineId come from the refund's metadata: webhooks carry no trace context (C1).
public sealed record RefundState(Guid OrderId, Guid OrderLineId, RefundStatus Status);

// Refund handling reacts to the status, not the webhook event name (D14).
public enum RefundStatus
{
    // Also covers a refund that still requires action.
    Pending,
    Succeeded,
    Failed,
    Canceled
}
