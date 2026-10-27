using System.Linq.Expressions;
using LiveLearn.Enrollment.Application.Repositories;
using LiveLearn.Enrollment.Domain.Entities;
using LiveLearn.Enrollment.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Enrollment.Infrastructure.Repositories;


internal sealed class EnrollmentRepository(WriteDbContext dbContext) : IEnrollmentRepository
{
    public async Task AddAsync(CourseEnrollment entity, CancellationToken ct = default)
    {
        await dbContext.CourseEnrollments.AddAsync(entity, ct);
    }

    public void Delete(CourseEnrollment entity)
    {
        dbContext.CourseEnrollments.Remove(entity);
    }

    public async Task<IEnumerable<CourseEnrollment>> FindAsync(Expression<Func<CourseEnrollment, bool>> predicate, CancellationToken ct = default)
    {
        return await dbContext.CourseEnrollments
            .Include(e => e.Items)
            .Where(predicate)
            .ToListAsync(ct);
    }

    public async Task<CourseEnrollment?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.CourseEnrollments
            .Include(e => e.Items)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<CourseEnrollment?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken ct = default)
    {
        return await dbContext.CourseEnrollments
            .Include(e => e.Items)
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId, ct);
    }

    public void Update(CourseEnrollment entity)
    {
        dbContext.CourseEnrollments.Update(entity);
    }
}
