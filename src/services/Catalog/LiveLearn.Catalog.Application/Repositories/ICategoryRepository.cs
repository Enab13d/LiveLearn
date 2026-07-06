using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.Entities;

namespace LiveLearn.Catalog.Application.Repositories;


public interface ICategoryRepository : IRepository<Category, Guid>
{
    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}
