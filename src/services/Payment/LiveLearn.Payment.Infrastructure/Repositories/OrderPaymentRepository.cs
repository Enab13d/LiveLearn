using System.Linq.Expressions;
using LiveLearn.Payment.Application.Repositories;
using LiveLearn.Payment.Domain.Entities;
using LiveLearn.Payment.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Payment.Infrastructure.Repositories;


// No unique-violation or concurrency translation: both propagate, and the entry point's retry re-runs
// the handler's guard on fresh state (D9).
internal sealed class OrderPaymentRepository(WriteDbContext dbContext) : IOrderPaymentRepository
{
    public async Task AddAsync(OrderPayment entity, CancellationToken ct = default)
    {
        await dbContext.OrderPayments.AddAsync(entity, ct);
    }

    public void Delete(OrderPayment entity)
    {
        dbContext.OrderPayments.Remove(entity);
    }

    public async Task<IEnumerable<OrderPayment>> FindAsync(Expression<Func<OrderPayment, bool>> predicate, CancellationToken ct = default)
    {
        return await dbContext.OrderPayments
            .Include(e => e.Lines)
            .Where(predicate)
            .ToListAsync(ct);
    }

    public async Task<OrderPayment?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.OrderPayments
            .Include(e => e.Lines)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public void Update(OrderPayment entity)
    {
        dbContext.OrderPayments.Update(entity);
    }
}
