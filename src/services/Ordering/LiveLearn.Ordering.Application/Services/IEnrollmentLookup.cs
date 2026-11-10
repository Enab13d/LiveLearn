namespace LiveLearn.Ordering.Application.Services;

// Access from Ordering's local Enrollment projection (O4, O9).
public interface IEnrollmentLookup
{
    // The subset of courseIds the student currently has active access to.
    Task<IReadOnlySet<Guid>> GetEnrolledCourseIdsAsync(Guid studentId, IReadOnlyCollection<Guid> courseIds, CancellationToken ct);
}
