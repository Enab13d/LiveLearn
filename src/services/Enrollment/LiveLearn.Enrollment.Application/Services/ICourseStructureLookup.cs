using LiveLearn.Enrollment.Domain.Common;

namespace LiveLearn.Enrollment.Application.Services;


public interface ICourseStructureLookup
{
    // Snapshot at the course's projected revision; null until CoursePublishedEvent has been applied.
    Task<CourseSnapshot?> GetSnapshotAsync(Guid courseId, CancellationToken ct = default);
}
