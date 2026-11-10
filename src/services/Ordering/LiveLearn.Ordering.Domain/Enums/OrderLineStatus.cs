namespace LiveLearn.Ordering.Domain.Enums;

// Pending → Enrolled → Refunded → RefundFailed, Pending → EnrollmentFailed → Refunded,
// Pending → Cancelled.
public enum OrderLineStatus
{
    Pending,
    Enrolled,
    EnrollmentFailed,
    Refunded,
    // Final: the refund didn't go through; access stays revoked.
    RefundFailed,
    Cancelled
}
