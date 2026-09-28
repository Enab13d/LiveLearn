using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Domain.DomainEvents;

namespace LiveLearn.Identity.Domain.Entities;

public sealed class User : AggregateRoot<Guid>
{
    private User() { }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    public string AvatarUrl { get; private set; } = string.Empty;

    public string Bio { get; private set; } = string.Empty;

    public static User Create(Guid id, string firstName, string lastName, string email)
    {
        var user = new User()
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            DisplayName = $"{firstName} {lastName}",
        };
        user.RaiseDomainEvent(new UserCreatedDomainEvent(id, email, user.DisplayName));
        return user;
    }

    public void UpdateProfile(string displayName, string? avatarUrl, string? bio)
    {
        DisplayName = displayName;
        AvatarUrl = avatarUrl ?? string.Empty;
        Bio = bio ?? string.Empty;
        RaiseDomainEvent(new UserProfileUpdatedDomainEvent(Id, DisplayName, AvatarUrl, Bio));
    }

    public void UpdateAuthData(string email)
    {
        Email = email;
        RaiseDomainEvent(new AuthDataUpdatedDomainEvent(Id, Email));
    }
}


