namespace LiveLearn.Contracts.Enrollment;

public sealed record EnrollmentRevokedEvent(
    Guid EnrollmentId,
    Guid StudentId,
    Guid CourseId,
    long Version,
    long CourseRevision,
    DateTimeOffset OccurredOn,
    string Reason
);
