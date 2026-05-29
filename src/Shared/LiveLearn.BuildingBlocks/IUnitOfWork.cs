namespace LiveLearn.BuildingBlocks;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
