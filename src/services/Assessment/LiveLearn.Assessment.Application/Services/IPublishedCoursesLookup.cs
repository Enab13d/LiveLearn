namespace LiveLearn.Assessment.Application.Services;

public interface IPublishedCoursesLookup
{
    Task<bool> IsPublishedAsync(Guid courseId, CancellationToken ct = default);
}
