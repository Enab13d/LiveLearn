namespace LiveLearn.Contracts.Payment;

// Stripe reported a refund of this line as failed after OrderLineRefunded was published. Final: access
// stays revoked and support returns the money by other means (D6).
public sealed record OrderLineRefundFailed(
    Guid OrderId,
    Guid OrderLineId,
    Guid CourseId,
    Guid StudentId,
    DateTimeOffset OccurredOn
);
