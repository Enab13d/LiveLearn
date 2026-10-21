using LiveLearn.Catalog.Domain.Common;

namespace LiveLearn.Catalog.Application.Services;


// Resolves how the current user may see a course. Returns null when the course does not exist.
public interface ICourseAccess
{
    // Browsing the course page: owner/admin → Full; anyone else → CurrentRevision if the course is published, otherwise None.
    public Task<CourseAccessContext?> GetBrowseAccessAsync(Guid courseId, CancellationToken ct = default);

    // Learning content: owner/admin → Full; active enrollment → EnrolledRevision regardless of the course status; otherwise None.
    public Task<CourseAccessContext?> GetLearningAccessAsync(Guid courseId, CancellationToken ct = default);
}
