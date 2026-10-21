namespace LiveLearn.Assessment.Application.Services;


public interface ICourseTaskLookup
{
    // True once the task was assigned to the course in any revision — unassigning never clears it.
    Task<bool> WasEverAssignedAsync(Guid courseId, Guid taskId, CancellationToken ct = default);
}
