using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Domain.DomainEvents;


public sealed record TaskDeletedDomainEvent(Guid TaskId) : IDomainEvent;
