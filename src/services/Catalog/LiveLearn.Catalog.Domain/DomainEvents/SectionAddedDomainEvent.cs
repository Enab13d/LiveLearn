using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.DomainEvents;


public sealed record SectionAddedDomainEvent(Guid CourseId, Guid SectionId, long Revision) : IDomainEvent;
