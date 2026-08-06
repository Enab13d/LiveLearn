using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.DomainEvents;


public sealed record SectionRemovedDomainEvent(Guid CourseId, Guid SectionId) : IDomainEvent;
