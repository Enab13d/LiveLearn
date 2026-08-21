using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.DomainEvents;
using LiveLearn.Contracts.Catalog;

namespace LiveLearn.Catalog.Application.DomainEventHandlers;

internal sealed class CoursePublishedDomainEventHandler(IEventBus eventBus) : IDomainEventHandler<CoursePublishedDomainEvent>
{
    public async Task Handle(CoursePublishedDomainEvent notification, CancellationToken ct)
    {
        await eventBus.PublishAsync<CoursePublishedEvent>(
            new(notification.CourseId, notification.TutorId, notification.TaskIds, notification.PublishedAt), ct);
    }
}
