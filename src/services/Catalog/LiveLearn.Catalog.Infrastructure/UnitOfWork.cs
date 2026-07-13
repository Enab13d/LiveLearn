using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Infrastructure.Contexts;

namespace LiveLearn.Catalog.Infrastructure;


internal sealed class UnitOfWork(WriteDbContext dbContext) : IUnitOfWork
{
    public async Task<int> CommitAsync(CancellationToken ct = default)
    {
        return await dbContext.SaveChangesAsync(ct);
    }
}
