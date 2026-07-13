using System.Linq.Expressions;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Domain.Entities;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Repositories;


internal sealed class CategoryRepository(WriteDbContext dbContext) : ICategoryRepository
{
    public async Task AddAsync(Category entity, CancellationToken ct = default)
    {
        await dbContext.Categories.AddAsync(entity, ct);
    }

    public void Delete(Category entity)
    {
        dbContext.Categories.Remove(entity);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Categories.AnyAsync(e => e.Id == id, ct);
    }

    public async Task<IEnumerable<Category>> FindAsync(Expression<Func<Category, bool>> predicate, CancellationToken ct = default)
    {
        return await dbContext.Categories.Where(predicate).ToListAsync(ct);
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Categories
            .FindAsync([id], ct);
    }

    public void Update(Category entity)
    {
        dbContext.Categories.Update(entity);
    }
}
