using LiveLearn.BuildingBlocks;

namespace LiveLearn.Identity.Domain.DomainEvents;


public sealed record AuthDataUpdatedDomainEvent(Guid UserId, string Email): IDomainEvent;
