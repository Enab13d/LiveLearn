using LiveLearn.BuildingBlocks;

namespace LiveLearn.Ordering.Domain.DomainEvents;


public sealed record OrderPlacedDomainEvent(Guid OrderId, Guid StudentId, DateTimeOffset PlacedAt) : IDomainEvent;
