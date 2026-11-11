using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Payment;
using LiveLearn.Payment.Domain.DomainEvents;

namespace LiveLearn.Payment.Application.DomainEventHandlers;


// Replies to the saga's RequestPayment; staged in the outbox and saved with the expired payment (D10).
internal sealed class OrderPaymentExpiredDomainEventHandler(IEventBus eventBus) : IDomainEventHandler<OrderPaymentExpiredDomainEvent>
{
    public async Task Handle(OrderPaymentExpiredDomainEvent notification, CancellationToken ct)
    {
        await eventBus.PublishAsync(new PaymentExpired(notification.OrderId, notification.ExpiredAt), ct);
    }
}
