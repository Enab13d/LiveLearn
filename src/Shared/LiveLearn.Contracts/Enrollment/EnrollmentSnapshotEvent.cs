namespace LiveLearn.Contracts.Enrollment;

// Republished for every enrollment on demand so new consumers can backfill.
public sealed record EnrollmentSnapshotEvent(
    Guid StudentId,
    Guid CourseId,
    string Status,
    long CourseRevision,
    long Version,
    DateTimeOffset OccurredOn
);
