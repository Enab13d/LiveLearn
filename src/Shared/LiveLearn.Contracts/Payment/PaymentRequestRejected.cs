namespace LiveLearn.Contracts.Payment;

// Reply to RequestPayment: no payment can be taken (invalid lines or a permanent Stripe error).
public sealed record PaymentRequestRejected(
    Guid OrderId,
    string Reason,
    DateTimeOffset OccurredOn
);
