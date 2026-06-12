using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Domain.DomainEvents;

namespace LiveLearn.Identity.Domain.Entities;

public sealed class User : AggregateRoot<Guid>
{
    private User() { }
    public string Email { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    public string AvatarUrl { get; private set; } = string.Empty;

    public string Bio { get; private set; } = string.Empty;

    public Role Role { get; private set; }

    public static User Create(Guid id, string email, string displayName, Role role = Role.Student)
    {
        var user = new User()
        {
            Id = id,
            Email = email,
            DisplayName = displayName,
            Role = role
        };
        user.RaiseDomainEvent(new UserCreatedDomainEvent(id, email, displayName));
        return user;
    }

    public void UpdateProfile(string displayName, string? avatarUrl, string? bio)
    {
        DisplayName = displayName;
        AvatarUrl = avatarUrl ?? string.Empty;
        Bio = bio ?? string.Empty;
        RaiseDomainEvent(new UserProfileUpdatedDomainEvent(Id, DisplayName, AvatarUrl, Bio));
    }
}


