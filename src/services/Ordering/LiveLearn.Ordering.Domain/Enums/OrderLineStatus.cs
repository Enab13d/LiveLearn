namespace LiveLearn.Ordering.Domain.Enums;

// Pending → Enrolled → Refunded → RefundFailed, Pending → EnrollmentFailed → Refunded → RefundFailed,
// Pending → Cancelled. RefundFailed may skip Refunded when its message arrives first (O6).
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
