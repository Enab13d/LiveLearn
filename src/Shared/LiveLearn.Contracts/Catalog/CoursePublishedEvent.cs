namespace LiveLearn.Contracts.Catalog;

public sealed record CoursePublishedEvent
(
    Guid CourseId,
    Guid TutorId,
    decimal Price,
    long Revision,
    IReadOnlyList<Guid> LectureIds,
    IReadOnlyList<Guid> TaskIds,
    DateTimeOffset PublishedAt
);
