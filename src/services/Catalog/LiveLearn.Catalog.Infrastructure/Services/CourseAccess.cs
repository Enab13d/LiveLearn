using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Domain.Common;
using LiveLearn.Catalog.Domain.Enums;
using LiveLearn.Catalog.Infrastructure.Contexts;
using LiveLearn.Catalog.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Services;


internal sealed class CourseAccess(ReadDbContext dbContext, ICurrentUser currentUser) : ICourseAccess
{
    public async Task<CourseAccessContext?> GetBrowseAccessAsync(Guid courseId, CancellationToken ct = default)
    {
        var course = await dbContext.Courses
            .Where(c => c.Id == courseId)
            .Select(c => new { c.TutorId, c.Status })
            .FirstOrDefaultAsync(ct);

        if (course is null) return null;
        if (IsPrivileged(course.TutorId)) return CourseAccessContext.Full;

        return course.Status == CourseStatus.Published.ToString()
            ? CourseAccessContext.CurrentRevision
            : CourseAccessContext.None;
    }

    public async Task<CourseAccessContext?> GetLearningAccessAsync(Guid courseId, CancellationToken ct = default)
    {
        var userId = currentUser.UserId;

        var course = await dbContext.Courses
            .Where(c => c.Id == courseId)
            .Select(c => new
            {
                c.TutorId,
                EnrolledRevision = dbContext.CourseEnrollments
                    .Where(e => e.CourseId == courseId && e.StudentId == userId && e.Status == CourseEnrollmentStatus.Active)
                    .Select(e => (long?)e.CourseRevision)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(ct);

        if (course is null) return null;
        if (IsPrivileged(course.TutorId)) return CourseAccessContext.Full;

        return course.EnrolledRevision is long revision
            ? CourseAccessContext.EnrolledRevision(revision)
            : CourseAccessContext.None;
    }

    private bool IsPrivileged(Guid tutorId) => currentUser.IsAdmin || tutorId == currentUser.UserId;
}
