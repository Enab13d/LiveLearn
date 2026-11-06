namespace LiveLearn.Contracts.Enrollment;

public sealed record EnrollmentActivatedEvent(
    Guid EnrollmentId,
    Guid StudentId,
    Guid CourseId,
    long Version,
    long CourseRevision,
    DateTimeOffset OccurredOn,
    Guid? OrderId,      // the order line that granted access; null for admin/backfill paths
    Guid? OrderLineId
);
