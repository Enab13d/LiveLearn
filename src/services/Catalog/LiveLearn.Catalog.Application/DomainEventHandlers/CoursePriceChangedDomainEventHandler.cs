using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.DomainEvents;
using LiveLearn.Contracts.Catalog;

namespace LiveLearn.Catalog.Application.DomainEventHandlers;


internal sealed class CoursePriceChangedDomainEventHandler(IEventBus eventBus, TimeProvider timeProvider) : IDomainEventHandler<CoursePriceChangedDomainEvent>
{
    public async Task Handle(CoursePriceChangedDomainEvent notification, CancellationToken ct)
    {
        await eventBus.PublishAsync(
            new CoursePriceChangedEvent(notification.CourseId, notification.NewPrice, timeProvider.GetUtcNow()), ct);
    }
}
