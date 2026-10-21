using LiveLearn.Assessment.Application.Services;
using LiveLearn.Assessment.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.Services;


public sealed class CourseTaskLookup(WriteDbContext dbContext) : ICourseTaskLookup
{
    public async Task<bool> WasEverAssignedAsync(Guid courseId, Guid taskId, CancellationToken ct = default)
    {
        return await dbContext.CourseTasks.AnyAsync(e => e.CourseId == courseId && e.TaskId == taskId, ct);
    }
}
