namespace LiveLearn.Catalog.Application.Services;


// Shared owner OR admin OR enrolled rule for lecture content and watch tickets.
public interface ICourseContentAccess
{
    public Task<bool> CanAccessAsync(Guid userId, bool isAdmin, Guid tutorId, Guid courseId, CancellationToken ct = default);
}
