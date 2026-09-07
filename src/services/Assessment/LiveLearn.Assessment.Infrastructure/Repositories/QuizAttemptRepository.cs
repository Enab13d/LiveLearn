using System.Linq.Expressions;
using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Domain.Entities;
using LiveLearn.Assessment.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.Repositories;


internal sealed class QuizAttemptRepository(WriteDbContext dbContext) : IQuizAttemptRepository
{
    public async Task AddAsync(QuizAttempt entity, CancellationToken ct = default)
    {
        await dbContext.QuizAttempts.AddAsync(entity, ct);
    }

    public void Delete(QuizAttempt entity)
    {
        dbContext.QuizAttempts.Remove(entity);
    }

    public async Task<IEnumerable<QuizAttempt>> FindAsync(Expression<Func<QuizAttempt, bool>> predicate, CancellationToken ct = default)
    {
        return await dbContext.QuizAttempts.Where(predicate).ToListAsync(ct);
    }

    public async Task<QuizAttempt?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.QuizAttempts.FindAsync([id], ct);
    }

    public void Update(QuizAttempt entity)
    {
        dbContext.QuizAttempts.Update(entity);
    }
}
