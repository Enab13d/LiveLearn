using LiveLearn.BuildingBlocks;

namespace LiveLearn.Identity.Domain.DomainEvents;

public record UserProfileUpdatedDomainEvent(
    Guid Id,
    string DisplayName,
    string? AvatarUrl,
    string? Bio) : IDomainEvent;
