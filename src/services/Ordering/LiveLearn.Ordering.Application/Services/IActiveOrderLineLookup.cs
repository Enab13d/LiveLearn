namespace LiveLearn.Ordering.Application.Services;

// The student's active (Pending / Enrolled) order lines (O4). At most one per course — the partial unique index (O5).
public interface IActiveOrderLineLookup
{
    // CourseId → OrderId of the active line; courses without one are absent.
    Task<IReadOnlyDictionary<Guid, Guid>> GetOrderIdsByCourseAsync(Guid studentId, IReadOnlyCollection<Guid> courseIds, CancellationToken ct);
}
