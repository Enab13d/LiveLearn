using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.DomainEvents;
using LiveLearn.Contracts.Catalog;

namespace LiveLearn.Catalog.Application.DomainEventHandlers;


public sealed class TaskRemovedFromSectionDomainEventHandler(IEventBus eventBus, TimeProvider timeProvider) : IDomainEventHandler<TaskRemovedFromSectionDomainEvent>
{
    public async Task Handle(TaskRemovedFromSectionDomainEvent notification, CancellationToken ct)
    {
        await eventBus.PublishAsync<TaskRemovedFromSectionEvent>(
            new(notification.TaskId, notification.SectionId, notification.CourseId, timeProvider.GetUtcNow()), ct);
    }
}
