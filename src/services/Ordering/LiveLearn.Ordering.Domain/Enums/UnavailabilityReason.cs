namespace LiveLearn.Ordering.Domain.Enums;

// Why a course can't be bought right now (O4).
public enum UnavailabilityReason
{
    // Not in the projection, not Published, or not priced above zero.
    Unpublished,
    AlreadyEnrolled,
    OwnCourse,
    // An active (Pending / Enrolled) line of another order already covers the course.
    PendingOrder
}
