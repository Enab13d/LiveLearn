using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.DomainEvents;

public sealed record CoursePublishedDomainEvent(
    Guid CourseId,
    Guid TutorId,
    IReadOnlyList<Guid> TaskIds,
    DateTimeOffset PublishedAt) : IDomainEvent;
