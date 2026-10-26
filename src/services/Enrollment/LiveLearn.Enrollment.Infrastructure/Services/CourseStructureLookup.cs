using LiveLearn.Enrollment.Application.Services;
using LiveLearn.Enrollment.Domain.Common;
using LiveLearn.Enrollment.Domain.Enums;
using LiveLearn.Enrollment.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Enrollment.Infrastructure.Services;


public sealed class CourseStructureLookup(WriteDbContext dbContext) : ICourseStructureLookup
{
    public async Task<CourseSnapshot?> GetSnapshotAsync(Guid courseId, CancellationToken ct = default)
    {
        // One statement, so the revision and the items come from the same committed state.
        var course = await dbContext.CourseRevisions
            .AsNoTracking()
            .Where(e => e.CourseId == courseId)
            .Select(e => new
            {
                e.Revision,
                Items = dbContext.CourseStructureItems
                    .Where(i => i.CourseId == e.CourseId)
                    .Select(i => new { i.ItemId, i.Kind })
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (course is null) return null;

        return new CourseSnapshot(
            course.Revision,
            course.Items.Where(e => e.Kind == EnrollmentItemKind.Lecture).Select(e => e.ItemId).ToList(),
            course.Items.Where(e => e.Kind == EnrollmentItemKind.Task).Select(e => e.ItemId).ToList());
    }
}
