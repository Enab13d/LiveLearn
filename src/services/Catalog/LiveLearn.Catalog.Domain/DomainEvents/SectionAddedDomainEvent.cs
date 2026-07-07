using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.DomainEvents;


public sealed record SectionAddedDomainEvent(Guid SectionId, Guid CourseId): IDomainEvent;
