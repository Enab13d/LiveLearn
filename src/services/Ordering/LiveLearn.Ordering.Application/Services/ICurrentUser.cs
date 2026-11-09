namespace LiveLearn.Ordering.Application.Services;


// The caller of the current HTTP request (O2: StudentId = sub). Not available in message consumers —
// commands sent from consumers carry the user id explicitly.
public interface ICurrentUser
{
    // Guid.Empty for anonymous callers.
    public Guid UserId { get; }
}
