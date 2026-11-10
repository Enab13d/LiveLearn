using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Domain.Common;
using LiveLearn.Payment.Domain.DomainEvents;
using LiveLearn.Payment.Domain.Enums;
using LiveLearn.Payment.Domain.Errors;

namespace LiveLearn.Payment.Domain.Entities;


// One payment per order: one Checkout Session, one charge (D1, D13).
public sealed class OrderPayment : AggregateRoot<Guid>
{
    public Guid OrderId { get; private set; }

    public Guid StudentId { get; private set; }

    public OrderPaymentStatus Status { get; private set; }

    public Money Total { get; private set; } = null!;

    // Fixes the Checkout Session's expiry, so a retried create sends the same parameters under its
    // idempotency key (D14).
    public DateTimeOffset CreatedAt { get; private set; }

    public string? CheckoutSessionId { get; private set; }

    public string? CheckoutUrl { get; private set; }

    public string? ChargeId { get; private set; }

    // Stripe's fee for the charge; LiveLearn pays it in full (D5).
    public Money? Fee { get; private set; }

    public DateTimeOffset? PaidAt { get; private set; }

    // PaidAt + the refund window, snapshotted so a later option change never moves it (D6).
    public DateTimeOffset? RefundWindowEndsAt { get; private set; }

    private readonly List<OrderPaymentLine> _lines = [];

    public IReadOnlyCollection<OrderPaymentLine> Lines => _lines.AsReadOnly();

    private OrderPayment() { }

    public static Result<OrderPayment> Create(Guid orderId, Guid studentId, IReadOnlyCollection<OrderPaymentLineRequest> lines, decimal tutorShareRate, DateTimeOffset createdAt)
    {
        if (lines.Count == 0)
            return Result<OrderPayment>.Failure(OrderPaymentErrors.NoLines);

        var payment = new OrderPayment()
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            StudentId = studentId,
            Status = OrderPaymentStatus.AwaitingPayment,
            CreatedAt = createdAt,
        };

        foreach (var line in lines)
        {
            var price = Money.Create(line.Price, line.Currency);
            if (!price.IsSuccess)
                return Result<OrderPayment>.Failure(price.Errors);

            if (price.Value.Amount <= 0)
                return Result<OrderPayment>.Failure(OrderPaymentErrors.NonPositivePrice);

            payment._lines.Add(OrderPaymentLine.Create(payment.Id, line.OrderLineId, line.CourseId, line.TutorId, line.CourseTitle, price.Value, tutorShareRate));
        }

        // Money supports USD only, so all lines share one currency and the sum can't mix currencies.
        payment.Total = payment._lines.Skip(1).Aggregate(payment._lines[0].Price, (sum, e) => sum + e.Price);

        return payment;
    }

    // The session is created once (idempotency key OrderId); a redelivered attach changes nothing.
    public void AttachCheckoutSession(string sessionId, string url)
    {
        if (Status != OrderPaymentStatus.AwaitingPayment || CheckoutSessionId is not null) return;

        CheckoutSessionId = sessionId;
        CheckoutUrl = url;
    }

    public void MarkPaid(string chargeId, Money fee, DateTimeOffset paidAt, TimeSpan refundWindow)
    {
        if (Status != OrderPaymentStatus.AwaitingPayment) return;

        Status = OrderPaymentStatus.Paid;
        ChargeId = chargeId;
        Fee = fee;
        PaidAt = paidAt;
        RefundWindowEndsAt = paidAt + refundWindow;

        RaiseDomainEvent(new OrderPaymentPaidDomainEvent(Id, OrderId, _lines.Select(e => e.OrderLineId).ToList(), paidAt));
    }

    public void Expire(DateTimeOffset expiredAt)
    {
        if (Status != OrderPaymentStatus.AwaitingPayment) return;

        Status = OrderPaymentStatus.Expired;

        RaiseDomainEvent(new OrderPaymentExpiredDomainEvent(Id, OrderId, expiredAt));
    }
}
