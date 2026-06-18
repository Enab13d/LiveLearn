using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Domain.Entities;

namespace LiveLearn.Identity.Domain.DomainEvents;


public sealed record AuthDataUpdatedDomainEvent(string Email, Role role): IDomainEvent;
