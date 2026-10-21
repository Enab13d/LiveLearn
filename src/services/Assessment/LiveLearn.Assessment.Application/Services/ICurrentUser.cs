namespace LiveLearn.Assessment.Application.Services;


// The caller of the current HTTP request. Not available in message consumers — commands sent from
// consumers carry the user id explicitly.
public interface ICurrentUser
{
    // Guid.Empty for anonymous callers.
    public Guid UserId { get; }

    public bool IsAdmin { get; }
}
