using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.DomainEvents;

public sealed record CoursePublishedDomainEvent(
    Guid CourseId,
    Guid TutorId,
    decimal Price,
    long Revision,
    IReadOnlyList<Guid> LectureIds,
    IReadOnlyList<Guid> TaskIds,
    DateTimeOffset PublishedAt) : IDomainEvent;
