using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Domain.Common;
using LiveLearn.Ordering.Domain.Enums;

namespace LiveLearn.Ordering.Domain.Errors;


public static class CartErrors
{
    private static readonly string _prefix = nameof(CartErrors);

    public static Error AlreadyInCart => Error.Conflict($"{_prefix}.AlreadyInCart", "Course is already in the cart");

    public static Error CartFull => Error.Conflict($"{_prefix}.CartFull", "Cart has reached its item limit");

    public static Error Unavailable(CourseAvailability availability) => availability.Reason switch
    {
        UnavailabilityReason.Unpublished => Error.NotFound($"{_prefix}.Unpublished", "Course is not available for purchase"),
        UnavailabilityReason.OwnCourse => Error.Conflict($"{_prefix}.OwnCourse", "You can't buy your own course"),
        UnavailabilityReason.AlreadyEnrolled => Error.Conflict($"{_prefix}.AlreadyEnrolled", "You are already enrolled in this course"),
        UnavailabilityReason.PendingOrder => Error.Conflict($"{_prefix}.PendingOrder",
            $"Course is already in order {availability.PendingOrderId}"),
        _ => throw new ArgumentOutOfRangeException(nameof(availability), availability.Reason, "Course is available")
    };
}
