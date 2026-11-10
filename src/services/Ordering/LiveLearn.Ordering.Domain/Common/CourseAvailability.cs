using LiveLearn.Ordering.Domain.Enums;

namespace LiveLearn.Ordering.Domain.Common;

// Result of PurchaseRules: Available, or Unavailable with a reason (+ the blocking order for PendingOrder).
public sealed record CourseAvailability
{
    public UnavailabilityReason? Reason { get; }

    public Guid? PendingOrderId { get; }

    public bool IsAvailable => Reason is null;

    private CourseAvailability(UnavailabilityReason? reason, Guid? pendingOrderId)
    {
        Reason = reason;
        PendingOrderId = pendingOrderId;
    }

    public static CourseAvailability Available { get; } = new(null, null);

    public static CourseAvailability Unavailable(UnavailabilityReason reason, Guid? pendingOrderId = null) =>
        new(reason, pendingOrderId);
}
