using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Domain.DomainEvents;


public sealed record TaskCreatedDomainEvent(Guid TaskId, Guid TutorId, string TaskType) : IDomainEvent;
