namespace LiveLearn.Contracts.Payment;

// Command sent by the Ordering saga to Payment's queue once an order is placed. Idempotent per OrderId:
// a redelivery or resend never creates a second payment (D10).
public sealed record RequestPayment(
    Guid OrderId,
    Guid StudentId,
    IReadOnlyList<RequestPaymentLine> Lines
);

// One order line to charge. CourseTitle names the Checkout line item the student sees; Price and
// Currency are as placed, and Payment validates them (D8).
public sealed record RequestPaymentLine(
    Guid OrderLineId,
    Guid CourseId,
    Guid TutorId,
    string CourseTitle,
    decimal Price,
    string Currency
);
