namespace LiveLearn.Contracts.Payment;

// Reply to RequestPayment: the order's charge succeeded. OrderLineIds are every line in the payment.
public sealed record PaymentSucceeded(
    Guid OrderId,
    IReadOnlyList<Guid> OrderLineIds,
    DateTimeOffset OccurredOn
);
