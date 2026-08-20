using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Services;


public interface ITaskReferenceLookup
{
    public Task<TaskReferenceDto?> GetByTaskIdAsync(Guid taskId, CancellationToken ct = default);
}
