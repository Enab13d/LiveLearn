using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.DomainEvents;


public sealed record CoursePriceChangedDomainEvent(Guid CourseId, decimal NewPrice) : IDomainEvent;
