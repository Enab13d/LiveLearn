using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.DomainEvents;


public sealed record TaskAssignedToSectionDomainEvent(
    Guid TaskId,
    Guid SectionId, 
    Guid CourseId
) : IDomainEvent;
