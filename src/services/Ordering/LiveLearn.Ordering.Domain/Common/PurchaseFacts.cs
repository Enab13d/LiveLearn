namespace LiveLearn.Ordering.Domain.Common;

// Everything the O4 rules need about one course for one student.
// Course is null when the projection has no row; PendingOrderId is the order of an active (Pending / Enrolled) line.
public sealed record PurchaseFacts(CourseFacts? Course, bool IsEnrolled, Guid? PendingOrderId);
