using LiveLearn.Assessment.Application.Services;
using LiveLearn.Assessment.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.Services;


public sealed class LockedTaskLookup(WriteDbContext dbContext) : ILockedTaskLookup
{
    public async Task<bool> IsLockedAsync(Guid taskId, CancellationToken ct = default)
    {
        return await dbContext.LockedTasks.AnyAsync(e => e.TaskId == taskId, ct);
    }
}
