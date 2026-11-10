using LiveLearn.Ordering.Application.Services;
using LiveLearn.Ordering.Domain.Enums;
using LiveLearn.Ordering.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Ordering.Infrastructure.Services;


internal sealed class ActiveOrderLineLookup(ReadDbContext dbContext) : IActiveOrderLineLookup
{
    // The statuses covered by the partial unique index (O5).
    private static readonly string[] ActiveStatuses = [nameof(OrderLineStatus.Pending), nameof(OrderLineStatus.Enrolled)];

    public async Task<IReadOnlyDictionary<Guid, Guid>> GetOrderIdsByCourseAsync(Guid studentId, IReadOnlyCollection<Guid> courseIds, CancellationToken ct)
    {
        if (courseIds.Count == 0) return new Dictionary<Guid, Guid>();

        return await dbContext.OrderLines
            .Where(l => l.StudentId == studentId && ActiveStatuses.Contains(l.Status) && courseIds.Contains(l.CourseId))
            .ToDictionaryAsync(l => l.CourseId, l => l.OrderId, ct);
    }
}
