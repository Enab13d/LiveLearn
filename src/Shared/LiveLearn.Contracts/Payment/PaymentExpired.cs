namespace LiveLearn.Contracts.Payment;

// Reply to RequestPayment: the Checkout Session expired unpaid; the order is cancelled.
public sealed record PaymentExpired(
    Guid OrderId,
    DateTimeOffset OccurredOn
);
