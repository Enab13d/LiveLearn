using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Domain.Common;
using LiveLearn.Ordering.Domain.Enums;

namespace LiveLearn.Ordering.Domain.Entities;


public sealed class OrderLine : Entity<Guid>
{
    public Guid OrderId { get; private set; }

    // Denormalized from the order so the partial unique index (StudentId, CourseId) can live on this table (O5).
    public Guid StudentId { get; private set; }

    public Guid CourseId { get; private set; }

    // Course owner at placement.
    public Guid TutorId { get; private set; }

    public decimal Price { get; private set; }

    public OrderLineStatus Status { get; private set; }

    // Enrolled, or EnrollmentFailed with its refund requested, or refunded / refund failed (O6).
    public bool IsFinal => Status is OrderLineStatus.Enrolled or OrderLineStatus.EnrollmentFailed
        or OrderLineStatus.Refunded or OrderLineStatus.RefundFailed;

    private OrderLine() { }

    internal static OrderLine Create(Guid orderId, Guid studentId, OrderLineSnapshot snapshot)
    {
        return new OrderLine()
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            StudentId = studentId,
            CourseId = snapshot.CourseId,
            TutorId = snapshot.TutorId,
            Price = snapshot.Price,
            Status = OrderLineStatus.Pending,
        };
    }

    // Each transition applies only from its source status; anything else is a no-op, so redelivered
    // or out-of-order messages (e.g. a late payment's refund of a Cancelled line) change nothing.
    internal void MarkEnrolled()
    {
        if (Status == OrderLineStatus.Pending) Status = OrderLineStatus.Enrolled;
    }

    internal void MarkEnrollmentFailed()
    {
        if (Status == OrderLineStatus.Pending) Status = OrderLineStatus.EnrollmentFailed;
    }

    internal void MarkRefunded()
    {
        // A student refunds only an Enrolled line; the saga refunds an EnrollmentFailed one.
        if (Status is OrderLineStatus.Enrolled or OrderLineStatus.EnrollmentFailed)
            Status = OrderLineStatus.Refunded;
    }

    internal void MarkRefundFailed()
    {
        // Not only from Refunded: OrderLineRefundFailed can overtake OrderLineRefunded, and it is published
        // only for a confirmed refund, so the line skips Refunded; the late refund then finds a final line.
        if (Status is OrderLineStatus.Enrolled or OrderLineStatus.EnrollmentFailed or OrderLineStatus.Refunded)
            Status = OrderLineStatus.RefundFailed;
    }

    internal void Cancel()
    {
        if (Status == OrderLineStatus.Pending) Status = OrderLineStatus.Cancelled;
    }
}
