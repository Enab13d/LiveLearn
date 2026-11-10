using LiveLearn.Ordering.Application.Services;
using LiveLearn.Ordering.Domain.Common;
using LiveLearn.Ordering.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Ordering.Infrastructure.Services;


internal sealed class CourseProjectionLookup(ReadDbContext dbContext) : ICourseProjectionLookup
{
    public async Task<IReadOnlyDictionary<Guid, CourseFacts>> GetByIdsAsync(IReadOnlyCollection<Guid> courseIds, CancellationToken ct)
    {
        if (courseIds.Count == 0) return new Dictionary<Guid, CourseFacts>();

        return await dbContext.CourseProjections
            .Where(c => courseIds.Contains(c.CourseId))
            .Select(c => new CourseFacts(c.CourseId, c.OwnerId, c.Price, c.Status))
            .ToDictionaryAsync(c => c.CourseId, ct);
    }
}
