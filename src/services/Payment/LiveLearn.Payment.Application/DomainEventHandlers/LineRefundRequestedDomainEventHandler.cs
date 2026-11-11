using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Application.Messages;
using LiveLearn.Payment.Domain.DomainEvents;

namespace LiveLearn.Payment.Application.DomainEventHandlers;


// Stages the Stripe refund in the outbox; it runs only once the request is saved (D14).
internal sealed class LineRefundRequestedDomainEventHandler(IEventBus eventBus) : IDomainEventHandler<LineRefundRequestedDomainEvent>
{
    public async Task Handle(LineRefundRequestedDomainEvent notification, CancellationToken ct)
    {
        await eventBus.PublishAsync(new IssueLineRefund(notification.OrderId, notification.OrderLineId), ct);
    }
}
