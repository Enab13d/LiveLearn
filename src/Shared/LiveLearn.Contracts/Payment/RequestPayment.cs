namespace LiveLearn.Contracts.Payment;

// Command sent by the Ordering saga to Payment's queue once an order is placed. Idempotent per OrderId:
// a redelivery or resend never creates a second payment (D10).
public sealed record RequestPayment(
    Guid OrderId,
    Guid StudentId,
    IReadOnlyList<RequestPaymentLine> Lines
);

// One order line to charge. Price and Currency are as placed; Payment validates them (D8).
public sealed record RequestPaymentLine(
    Guid OrderLineId,
    Guid CourseId,
    Guid TutorId,
    decimal Price,
    string Currency
);
