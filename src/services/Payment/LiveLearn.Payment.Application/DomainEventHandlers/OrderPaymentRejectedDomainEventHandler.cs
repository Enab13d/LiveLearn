using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Payment;
using LiveLearn.Payment.Domain.DomainEvents;

namespace LiveLearn.Payment.Application.DomainEventHandlers;


// Replies to the saga's RequestPayment; staged in the outbox and saved with the rejected payment (D10).
internal sealed class OrderPaymentRejectedDomainEventHandler(IEventBus eventBus) : IDomainEventHandler<OrderPaymentRejectedDomainEvent>
{
    public async Task Handle(OrderPaymentRejectedDomainEvent notification, CancellationToken ct)
    {
        await eventBus.PublishAsync(new PaymentRequestRejected(notification.OrderId, notification.Reason, notification.RejectedAt), ct);
    }
}
