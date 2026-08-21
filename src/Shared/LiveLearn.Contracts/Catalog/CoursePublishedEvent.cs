namespace LiveLearn.Contracts.Catalog;

public sealed record CoursePublishedEvent
(
    Guid CourseId,
    Guid TutorId,
    IReadOnlyList<Guid> TaskIds,
    DateTimeOffset PublishedAt
);
