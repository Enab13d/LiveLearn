using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Domain.Common;
using LiveLearn.Ordering.Domain.Enums;

namespace LiveLearn.Ordering.Domain.Entities;


public sealed class Order : AggregateRoot<Guid>
{
    public Guid StudentId { get; private set; }

    public DateTimeOffset PlacedAt { get; private set; }

    public OrderStatus Status { get; private set; }

    public decimal Total { get; private set; }

    private readonly List<OrderLine> _lines = [];

    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

    private Order() { }


    // The placement handler re-validates the cart and rejects an empty order (O5), so lines is never empty here.
    public static Order Create(Guid id, Guid studentId, DateTimeOffset placedAt, IReadOnlyCollection<OrderLineSnapshot> lines)
    {
        Order order = new()
        {
            Id = id,
            StudentId = studentId,
            PlacedAt = placedAt,
            Status = OrderStatus.AwaitingPayment,
            Total = lines.Sum(e => e.Price),
        };

        order._lines.AddRange(lines.Select(e => OrderLine.Create(id, studentId, e)));

        return order;
    }
}
