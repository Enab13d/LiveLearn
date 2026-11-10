using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Domain.Common;

namespace LiveLearn.Payment.Domain.Entities;


public sealed class OrderPaymentLine : Entity<Guid>
{
    public Guid OrderPaymentId { get; private set; }

    public Guid OrderLineId { get; private set; }

    public Guid CourseId { get; private set; }

    // Course owner at placement; receives the tutor share.
    public Guid TutorId { get; private set; }

    public Money Price { get; private set; } = null!;

    // Snapshotted at creation so a later rate change never touches existing sales (D5).
    public decimal TutorShareRate { get; private set; }

    private OrderPaymentLine() { }

    internal static OrderPaymentLine Create(Guid orderPaymentId, Guid orderLineId, Guid courseId, Guid tutorId, Money price, decimal tutorShareRate)
    {
        return new OrderPaymentLine()
        {
            Id = Guid.NewGuid(),
            OrderPaymentId = orderPaymentId,
            OrderLineId = orderLineId,
            CourseId = courseId,
            TutorId = tutorId,
            Price = price,
            TutorShareRate = tutorShareRate,
        };
    }
}
