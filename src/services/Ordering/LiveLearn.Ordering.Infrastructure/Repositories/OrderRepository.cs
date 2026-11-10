using System.Linq.Expressions;
using LiveLearn.Ordering.Application.Repositories;
using LiveLearn.Ordering.Domain.Entities;
using LiveLearn.Ordering.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Ordering.Infrastructure.Repositories;


// No unique-violation translation: a violation of the active-line index propagates, and the consumer
// retry re-runs the handler guard on fresh state (O6).
internal sealed class OrderRepository(WriteDbContext dbContext) : IOrderRepository
{
    public async Task AddAsync(Order entity, CancellationToken ct = default)
    {
        await dbContext.Orders.AddAsync(entity, ct);
    }

    public void Delete(Order entity)
    {
        dbContext.Orders.Remove(entity);
    }

    public async Task<IEnumerable<Order>> FindAsync(Expression<Func<Order, bool>> predicate, CancellationToken ct = default)
    {
        return await dbContext.Orders
            .Include(e => e.Lines)
            .Where(predicate)
            .ToListAsync(ct);
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Orders
            .Include(e => e.Lines)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public void Update(Order entity)
    {
        dbContext.Orders.Update(entity);
    }
}
