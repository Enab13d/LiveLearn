namespace LiveLearn.Contracts.Catalog;

public sealed record CoursePublishedEvent
(
    Guid CourseId,
    Guid TutorId,
    decimal Price,
    IReadOnlyList<Guid> TaskIds,
    DateTimeOffset PublishedAt
);
