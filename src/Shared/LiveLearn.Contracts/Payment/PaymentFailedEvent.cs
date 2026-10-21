namespace LiveLearn.Contracts.Payment;

public sealed record PaymentFailedEvent(
    Guid TransactionId,
    Guid StudentId,
    Guid CourseId,
    string Reason,
    DateTimeOffset OccurredOn
);
