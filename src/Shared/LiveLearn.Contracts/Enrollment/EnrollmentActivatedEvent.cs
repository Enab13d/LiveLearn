namespace LiveLearn.Contracts.Enrollment;

public sealed record EnrollmentActivatedEvent(
    Guid EnrollmentId,
    Guid StudentId,
    Guid CourseId,
    long Version,
    long CourseRevision,
    DateTimeOffset OccurredOn
);
