namespace LiveLearn.Contracts.Ordering;

// Published by the Ordering saga once a line's student is enrolled; lets Payment accept the student's
// refund request for it, so a revoke never reaches Enrollment before the enrollment (D6).
public sealed record OrderLineEnrolled(
    Guid OrderId,
    Guid OrderLineId,
    DateTimeOffset OccurredOn
);
