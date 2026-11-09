namespace LiveLearn.Ordering.Domain.Enums;

// Pending → Enrolled → Refunded, Pending → EnrollmentFailed, Pending → Cancelled,
// Pending / EnrollmentFailed → Refunded, Refunded → Enrolled on refund reversal (O6).
public enum OrderLineStatus
{
    Pending,
    Enrolled,
    EnrollmentFailed,
    Refunded,
    Cancelled
}
