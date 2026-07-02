using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.DomainEvents;

public sealed record CoursePublishedDomainEvent(Guid Id) : IDomainEvent;
