namespace LiveLearn.Ordering.Domain.Enums;

// AwaitingPayment → Paid → Settled, or AwaitingPayment → Cancelled.
public enum OrderStatus
{
    AwaitingPayment,
    Paid,
    // Every line is final: Enrolled, or EnrollmentFailed with its refund requested.
    Settled,
    Cancelled
}
