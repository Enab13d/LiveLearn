using System.Linq.Expressions;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Domain.Entities;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Repositories;


internal sealed class CourseRepository(WriteDbContext dbContext) : ICourseRepository
{
    public async Task AddAsync(Course entity, CancellationToken ct = default)
    {
        await dbContext.Courses.AddAsync(entity, ct);
    }

    public void Delete(Course entity)
    {
        dbContext.Courses.Remove(entity);
    }

    public async Task<IEnumerable<Course>> FindAsync(Expression<Func<Course, bool>> predicate, CancellationToken ct = default)
    {
        return await dbContext.Courses.Where(predicate).ToListAsync(ct);
    }

    public async Task<Course?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Courses
            .Include(e => e.Sections)
            .ThenInclude(e => e.Lectures)
            .Include(e => e.Sections)
            .ThenInclude(e => e.SectionTasks)
            .AsSplitQuery()
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<Course?> GetByTaskIdAsync(Guid taskId, CancellationToken ct = default)
    {
        return await dbContext.Courses
            .Include(e => e.Sections)
            .ThenInclude(e => e.SectionTasks)
            .Where(e => e.Sections
                .Any(e => e.SectionTasks
                    .Any(e => e.TaskId == taskId)))
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct);
    }

    public void Update(Course entity)
    {
        dbContext.Courses.Update(entity);
    }
}
