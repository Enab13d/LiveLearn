namespace LiveLearn.Payment.Application.Messages;

// Payment-internal: creates the Stripe refund of one line after the request is committed, never inside
// its transaction (D14). Published through the outbox together with the RefundRequested line.
public sealed record IssueLineRefund(Guid OrderId, Guid OrderLineId);
