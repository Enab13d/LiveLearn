namespace LiveLearn.Catalog.Application.Services;


internal sealed class CourseContentAccess(IEnrollmentLookup enrollmentLookup) : ICourseContentAccess
{
    public async Task<bool> CanAccessAsync(Guid userId, bool isAdmin, Guid tutorId, Guid courseId, CancellationToken ct = default)
    {
        if (isAdmin || tutorId == userId) return true;

        // TODO(US-CA05): also require the lecture to be visible at enrollment.CourseRevision
        var enrollment = await enrollmentLookup.GetActiveAsync(userId, courseId, ct);
        return enrollment is not null;
    }
}
