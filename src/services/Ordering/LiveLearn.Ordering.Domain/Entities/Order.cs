using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Domain.Common;
using LiveLearn.Ordering.Domain.Enums;
using LiveLearn.Ordering.Domain.Errors;

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

    public void MarkPaid()
    {
        if (Status != OrderStatus.AwaitingPayment) return;

        Status = OrderStatus.Paid;
    }

    // Students can't cancel; an unpaid order ends by payment expiry or rejection (O6).
    public void Cancel()
    {
        if (Status != OrderStatus.AwaitingPayment) return;

        Status = OrderStatus.Cancelled;
        foreach (var line in _lines) line.Cancel();
    }

    public Result<bool> MarkLineEnrolled(Guid lineId) => ChangeLine(lineId, e => e.MarkEnrolled());

    // Final at once: the saga requests the line's refund in the same step.
    public Result<bool> MarkLineEnrollmentFailed(Guid lineId) => ChangeLine(lineId, e => e.MarkEnrollmentFailed());

    public Result<bool> MarkLineRefunded(Guid lineId) => ChangeLine(lineId, e => e.MarkRefunded());

    // Stripe couldn't return the money; access stays revoked, support settles the refund by other means.
    public Result<bool> MarkLineRefundFailed(Guid lineId) => ChangeLine(lineId, e => e.MarkRefundFailed());

    // Returns whether every line is final; a paid order with all lines final becomes Settled.
    private Result<bool> ChangeLine(Guid lineId, Action<OrderLine> change)
    {
        var line = _lines.FirstOrDefault(e => e.Id == lineId);
        if (line is null) return Result<bool>.Failure(OrderErrors.LineNotFound);

        change(line);

        var allFinal = _lines.All(e => e.IsFinal);
        if (allFinal && Status == OrderStatus.Paid) Status = OrderStatus.Settled;

        return allFinal;
    }
}
