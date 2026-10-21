using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Domain.DomainEvents;


public sealed record TaskArchivedDomainEvent(Guid TaskId) : IDomainEvent;
