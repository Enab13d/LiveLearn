using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Infrastructure.Contexts;
using LiveLearn.Catalog.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Services;


internal sealed class EnrollmentLookup(WriteDbContext dbContext) : IEnrollmentLookup
{
    public async Task<ActiveEnrollmentDto?> GetActiveAsync(Guid studentId, Guid courseId, CancellationToken ct = default) =>
        await dbContext.CourseEnrollments
            .AsNoTracking()
            .Where(e =>
                e.StudentId == studentId
                && e.CourseId == courseId
                && e.Status == CourseEnrollmentStatus.Active)
            .Select(e => new ActiveEnrollmentDto(e.CourseRevision))
            .FirstOrDefaultAsync(ct);
}
