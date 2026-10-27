namespace LiveLearn.Identity.Application.Services;


// The caller of the current HTTP request. Not available in message consumers — commands sent from
// consumers carry the user id explicitly.
public interface ICurrentUser
{
    // Guid.Empty for anonymous callers.
    public Guid UserId { get; }

    // Empty string when the token does not carry the claim.
    public string Email { get; }

    public string FirstName { get; }

    public string LastName { get; }
}
