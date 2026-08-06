using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.DomainEvents;


public sealed record SectionUpdatedDomainEvent(Guid CourseId, Guid SectionId) : IDomainEvent;
