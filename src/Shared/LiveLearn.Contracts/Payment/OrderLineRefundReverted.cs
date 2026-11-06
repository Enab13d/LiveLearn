namespace LiveLearn.Contracts.Payment;

// Stripe reported a refund of this line as failed after OrderLineRefunded was published.
public sealed record OrderLineRefundReverted(
    Guid OrderId,
    Guid OrderLineId,
    Guid CourseId,
    Guid StudentId,
    DateTimeOffset OccurredOn
);
