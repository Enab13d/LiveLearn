using LiveLearn.Ordering.Domain.Common;

namespace LiveLearn.Ordering.Application.Services;

// Courses from Ordering's local Catalog projection (O4, O9).
public interface ICourseProjectionLookup
{
    // Courses without a projection row are absent from the result.
    Task<IReadOnlyDictionary<Guid, CourseFacts>> GetByIdsAsync(IReadOnlyCollection<Guid> courseIds, CancellationToken ct);
}
