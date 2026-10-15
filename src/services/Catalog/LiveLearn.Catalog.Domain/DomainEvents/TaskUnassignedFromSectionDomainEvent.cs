using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.DomainEvents;


public sealed record TaskUnassignedFromSectionDomainEvent(
    Guid TaskId,
    Guid SectionId,
    Guid CourseId,
    long Revision
) : IDomainEvent;
