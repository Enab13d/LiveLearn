namespace LiveLearn.Contracts.Payment;

public sealed record OrderLineRefunded(
    Guid OrderId,
    Guid OrderLineId,
    Guid CourseId,
    Guid StudentId,
    DateTimeOffset OccurredOn
);
