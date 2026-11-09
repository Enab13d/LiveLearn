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
}
