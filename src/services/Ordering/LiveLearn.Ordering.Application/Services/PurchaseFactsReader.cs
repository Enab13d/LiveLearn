using LiveLearn.Ordering.Domain.Common;

namespace LiveLearn.Ordering.Application.Services;

// Gathers the O4 inputs for many courses at once — one query per lookup — for PurchaseRules.Evaluate.
// Public because the cart query handler lives in Infrastructure.
public sealed class PurchaseFactsReader(
    ICourseProjectionLookup courseLookup,
    IEnrollmentLookup enrollmentLookup,
    IActiveOrderLineLookup activeOrderLineLookup)
{
    // One entry per requested course; a course missing from the projection gets Course = null.
    public async Task<IReadOnlyDictionary<Guid, PurchaseFacts>> ReadAsync(
        Guid studentId, IReadOnlyCollection<Guid> courseIds, CancellationToken ct)
    {
        var courses = await courseLookup.GetByIdsAsync(courseIds, ct);
        var enrolled = await enrollmentLookup.GetEnrolledCourseIdsAsync(studentId, courseIds, ct);
        var activeOrders = await activeOrderLineLookup.GetOrderIdsByCourseAsync(studentId, courseIds, ct);

        return courseIds.Distinct().ToDictionary(
            id => id,
            id => new PurchaseFacts(
                courses.GetValueOrDefault(id),
                enrolled.Contains(id),
                activeOrders.TryGetValue(id, out var orderId) ? orderId : null));
    }
}
