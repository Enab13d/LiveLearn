namespace LiveLearn.Contracts.Catalog;

public sealed record CoursePublishedEvent
(
    Guid CourseId,
    Guid TutorId,
    Guid CategoryId,
    string Title,
    decimal Price,
    DateTimeOffset PublishedAt
);
