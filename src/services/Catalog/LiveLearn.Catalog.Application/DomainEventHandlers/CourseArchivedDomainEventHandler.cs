using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.DomainEvents;
using LiveLearn.Contracts.Catalog;

namespace LiveLearn.Catalog.Application.DomainEventHandlers;

internal sealed class CourseArchivedDomainEventHandler(IEventBus eventBus) : IDomainEventHandler<CourseArchivedDomainEvent>
{
    public async Task Handle(CourseArchivedDomainEvent notification, CancellationToken ct)
    {
        await eventBus.PublishAsync<CourseArchivedEvent>(new(notification.CourseId), ct);
    }
}
