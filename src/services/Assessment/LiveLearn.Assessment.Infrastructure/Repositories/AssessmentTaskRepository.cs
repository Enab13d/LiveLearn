using System.Linq.Expressions;
using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Domain.Entities;
using LiveLearn.Assessment.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;


namespace LiveLearn.Assessment.Infrastructure.Repositories;

internal sealed class AssessmentTaskRepository(WriteDbContext dbContext) : IAssessmentTaskRepository
{
    public async Task AddAsync(AssessmentTask entity, CancellationToken ct = default)
    {
        await dbContext.AssessmentTasks.AddAsync(entity, ct);
    }

    public void Delete(AssessmentTask entity)
    {
        dbContext.AssessmentTasks.Remove(entity);
    }

    public async Task<IEnumerable<AssessmentTask>> FindAsync(Expression<Func<AssessmentTask, bool>> predicate, CancellationToken ct = default)
    {
        return await dbContext.AssessmentTasks.Where(predicate).ToListAsync(ct);
    }

    public async Task<AssessmentTask?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.AssessmentTasks.FindAsync([id], ct);
    }

    public async Task<Homework?> GetHomeworkAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Homeworks.FirstOrDefaultAsync(ct);
    }

    public async Task<Quiz?> GetQuizAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Quizzes.FirstOrDefaultAsync(ct);
    }

    public void Update(AssessmentTask entity)
    {
        dbContext.AssessmentTasks.Update(entity);
    }
}
