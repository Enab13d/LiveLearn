using LiveLearn.Assessment.Application.Services;
using LiveLearn.Assessment.Infrastructure.Contexts;
using LiveLearn.Assessment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.Services;


public sealed class EnrollmentLookup(WriteDbContext dbContext) : IEnrollmentLookup
{
    public async Task<bool> IsEnrolledAsync(Guid userId, Guid courseId, CancellationToken ct = default) =>
        await dbContext.Enrollments
            .AnyAsync(e => 
                e.StudentId == userId 
                && e.CourseId == courseId
                && e.Status == EnrollmentStatus.Active, ct);

}
