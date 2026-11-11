using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Application.Messages;
using LiveLearn.Payment.Domain.DomainEvents;

namespace LiveLearn.Payment.Application.DomainEventHandlers;


// Stages the Checkout Session creation in the outbox; it runs only once the payment is saved (D14).
internal sealed class OrderPaymentCreatedDomainEventHandler(IEventBus eventBus) : IDomainEventHandler<OrderPaymentCreatedDomainEvent>
{
    public async Task Handle(OrderPaymentCreatedDomainEvent notification, CancellationToken ct)
    {
        await eventBus.PublishAsync(new CreateCheckoutSession(notification.OrderId), ct);
    }
}
