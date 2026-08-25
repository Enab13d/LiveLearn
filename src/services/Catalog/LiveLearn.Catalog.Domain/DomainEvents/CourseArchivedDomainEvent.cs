using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.DomainEvents;


public sealed record CourseArchivedDomainEvent(Guid CourseId) : IDomainEvent;
