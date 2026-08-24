using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Services;


internal sealed class TaskReferenceLookup(WriteDbContext dbContext) : ITaskReferenceLookup
{
    public async Task<TaskReferenceDto?> GetByTaskIdAsync(Guid taskId, CancellationToken ct = default)
    {
        var taskReference = await dbContext.TaskReplicas.FirstOrDefaultAsync(e => e.TaskId == taskId, ct);
        
        return taskReference is null ? null : new TaskReferenceDto(taskReference.TaskId, taskReference.TutorId, taskReference.TaskType);

    }
}
