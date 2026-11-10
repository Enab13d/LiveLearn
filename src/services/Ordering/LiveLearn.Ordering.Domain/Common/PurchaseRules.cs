using LiveLearn.Ordering.Domain.Enums;

namespace LiveLearn.Ordering.Domain.Common;

// The only place for the O4 purchase rules, checked at add-to-cart and at placement.
// "Already in the cart" is not here — it applies only on add and lives in that handler.
public static class PurchaseRules
{
    public static CourseAvailability Evaluate(PurchaseFacts facts, Guid studentId)
    {
        if (facts.Course is not { Status: CourseStatus.Published, OwnerId: not null, Price: > 0 } course)
            return CourseAvailability.Unavailable(UnavailabilityReason.Unpublished);

        if (course.OwnerId == studentId)
            return CourseAvailability.Unavailable(UnavailabilityReason.OwnCourse);

        if (facts.IsEnrolled)
            return CourseAvailability.Unavailable(UnavailabilityReason.AlreadyEnrolled);

        if (facts.PendingOrderId is { } orderId)
            return CourseAvailability.Unavailable(UnavailabilityReason.PendingOrder, orderId);

        return CourseAvailability.Available;
    }
}
