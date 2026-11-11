using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Domain.Common;
using LiveLearn.Payment.Domain.Enums;

namespace LiveLearn.Payment.Domain.Entities;


public sealed class OrderPaymentLine : Entity<Guid>
{
    public Guid OrderPaymentId { get; private set; }

    public Guid OrderLineId { get; private set; }

    public Guid CourseId { get; private set; }

    // Course owner at placement; receives the tutor share.
    public Guid TutorId { get; private set; }

    // Title at placement; names the Checkout line item.
    public string CourseTitle { get; private set; } = null!;

    public Money Price { get; private set; } = null!;

    // Snapshotted at creation so a later rate change never touches existing sales (D5).
    public decimal TutorShareRate { get; private set; }

    public OrderPaymentLineStatus Status { get; private set; }

    // Set from Ordering's OrderLineEnrolled; a student may refund only an enrolled line, so the revoke
    // never reaches Enrollment before the enrollment (D6).
    public bool IsEnrolled { get; private set; }

    // Stripe refund of this line; set once the refund is confirmed.
    public string? RefundId { get; private set; }

    private OrderPaymentLine() { }

    internal static OrderPaymentLine Create(Guid orderPaymentId, Guid orderLineId, Guid courseId, Guid tutorId, string courseTitle, Money price, decimal tutorShareRate)
    {
        return new OrderPaymentLine()
        {
            Id = Guid.NewGuid(),
            OrderPaymentId = orderPaymentId,
            OrderLineId = orderLineId,
            CourseId = courseId,
            TutorId = tutorId,
            CourseTitle = courseTitle,
            Price = price,
            TutorShareRate = tutorShareRate,
            Status = OrderPaymentLineStatus.AwaitingPayment,
        };
    }

    internal void MarkPaid() => Status = OrderPaymentLineStatus.Paid;

    internal void MarkEnrolled() => IsEnrolled = true;

    internal void MarkRefundRequested() => Status = OrderPaymentLineStatus.RefundRequested;

    internal void MarkRefunded(string refundId)
    {
        Status = OrderPaymentLineStatus.Refunded;
        RefundId = refundId;
    }

    internal void MarkRefundFailed() => Status = OrderPaymentLineStatus.RefundFailed;
}
