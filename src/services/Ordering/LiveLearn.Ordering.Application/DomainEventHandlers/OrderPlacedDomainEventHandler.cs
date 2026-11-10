using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Application.Messages;
using LiveLearn.Ordering.Domain.DomainEvents;

namespace LiveLearn.Ordering.Application.DomainEventHandlers;


// Starts the purchase saga (O8): OrderPlaced is staged in the outbox and saved with the order.
internal sealed class OrderPlacedDomainEventHandler(IEventBus eventBus) : IDomainEventHandler<OrderPlacedDomainEvent>
{
    public async Task Handle(OrderPlacedDomainEvent notification, CancellationToken ct)
    {
        await eventBus.PublishAsync(new OrderPlaced(notification.OrderId, notification.StudentId, notification.PlacedAt), ct);
    }
}
