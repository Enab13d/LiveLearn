namespace LiveLearn.Contracts.Payment;

public sealed record PaymentConfirmedEvent(
    Guid TransactionId,
    Guid StudentId,
    Guid CourseId,
    decimal Amount,
    DateTimeOffset OccurredOn
);
