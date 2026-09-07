namespace LiveLearn.Assessment.Application.Services;

public interface ILockedTaskLookup
{
    Task<bool> IsLockedAsync(Guid taskId, CancellationToken ct = default);
}
