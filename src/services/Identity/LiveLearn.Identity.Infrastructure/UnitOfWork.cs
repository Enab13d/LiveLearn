using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Infrastructure.Context;

namespace LiveLearn.Identity.Infrastructure;


internal sealed class UnitOfWork(IdentityDbContext dbContext) : IUnitOfWork
{
    public async Task<int> CommitAsync(CancellationToken ct = default)
    {
        return await dbContext.SaveChangesAsync(ct);
    }
}
