using LiveLearn.Ordering.Application.Services;
using LiveLearn.Ordering.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Ordering.Infrastructure.Services;


internal sealed class EnrollmentLookup(ReadDbContext dbContext) : IEnrollmentLookup
{
    public async Task<IReadOnlySet<Guid>> GetEnrolledCourseIdsAsync(Guid studentId, IReadOnlyCollection<Guid> courseIds, CancellationToken ct)
    {
        if (courseIds.Count == 0) return new HashSet<Guid>();

        return await dbContext.EnrollmentProjections
            .Where(e => e.StudentId == studentId && e.IsActive && courseIds.Contains(e.CourseId))
            .Select(e => e.CourseId)
            .ToHashSetAsync(ct);
    }
}
