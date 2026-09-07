namespace LiveLearn.Contracts.Assessment;


public sealed record TaskCompletedEvent(
    Guid TaskId,
    Guid StudentId,
    Guid SectionId,
    Guid CourseId,
    DateTimeOffset OccurredOn
);
