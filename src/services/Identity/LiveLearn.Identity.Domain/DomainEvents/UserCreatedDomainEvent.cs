using LiveLearn.BuildingBlocks;

namespace LiveLearn.Identity.Domain.DomainEvents;


public record UserCreatedDomainEvent(Guid Id, string Email, string DisplayName) : IDomainEvent;
