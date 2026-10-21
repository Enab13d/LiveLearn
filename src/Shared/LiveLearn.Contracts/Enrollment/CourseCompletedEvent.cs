namespace LiveLearn.Contracts.Enrollment;

public sealed record CourseCompletedEvent(
    Guid StudentId,
    Guid CourseId,
    DateTimeOffset OccurredOn
);
