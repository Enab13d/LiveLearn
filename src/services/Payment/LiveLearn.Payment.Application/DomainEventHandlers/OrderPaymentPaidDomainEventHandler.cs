using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Payment;
using LiveLearn.Payment.Domain.DomainEvents;

namespace LiveLearn.Payment.Application.DomainEventHandlers;


// Replies to the saga's RequestPayment; staged in the outbox and saved with the paid payment (D10).
internal sealed class OrderPaymentPaidDomainEventHandler(IEventBus eventBus) : IDomainEventHandler<OrderPaymentPaidDomainEvent>
{
    public async Task Handle(OrderPaymentPaidDomainEvent notification, CancellationToken ct)
    {
        await eventBus.PublishAsync(new PaymentSucceeded(notification.OrderId, notification.OrderLineIds.ToList(), notification.PaidAt), ct);
    }
}
