using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.DomainEvents;


public sealed record TaskRemovedFromSectionDomainEvent(
    Guid TaskId,
    Guid SectionId,
    Guid CourseId
) : IDomainEvent;
