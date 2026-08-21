using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.DomainEvents;
using LiveLearn.Contracts.Catalog;

namespace LiveLearn.Catalog.Application.DomainEventHandlers;

internal sealed class SectionRemovedDomainEventHandler(IEventBus eventBus) : IDomainEventHandler<SectionRemovedDomainEvent>
{
    public async Task Handle(SectionRemovedDomainEvent notification, CancellationToken ct)
    {
        await eventBus.PublishAsync(
            new SectionRemovedEvent(
                notification.CourseId, notification.SectionId, notification.SectionTaskIds), ct);
    }
}
