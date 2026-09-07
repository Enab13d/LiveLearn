using System.Linq.Expressions;
using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Domain.Entities;
using LiveLearn.Assessment.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.Repositories;


internal sealed class HomeworkSubmissionRepository(WriteDbContext dbContext) : IHomeworkSubmissionRepository
{
    public async Task AddAsync(HomeworkSubmission entity, CancellationToken ct = default)
    {
        await dbContext.HomeworkSubmissions.AddAsync(entity, ct);
    }

    public void Delete(HomeworkSubmission entity)
    {
        dbContext.HomeworkSubmissions.Remove(entity);
    }

    public async Task<IEnumerable<HomeworkSubmission>> FindAsync(Expression<Func<HomeworkSubmission, bool>> predicate, CancellationToken ct = default)
    {
        return await dbContext.HomeworkSubmissions.Where(predicate).ToListAsync(ct);
    }

    public async Task<HomeworkSubmission?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.HomeworkSubmissions.FindAsync([id], ct);
    }

    public void Update(HomeworkSubmission entity)
    {
        dbContext.HomeworkSubmissions.Update(entity);
    }
}
