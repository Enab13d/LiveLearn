namespace LiveLearn.Contracts.Payment;

public sealed record RefundCompletedEvent(
    Guid StudentId,
    Guid CourseId,
    string Reason,
    DateTimeOffset OccurredOn
);
