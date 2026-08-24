using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.DomainEvents;
using LiveLearn.Contracts.Catalog;

namespace LiveLearn.Catalog.Application.DomainEventHandlers;


internal sealed class AssignTaskToSectionDomainEventHandler(IEventBus eventBus, TimeProvider timeProvider) 
    : IDomainEventHandler<TaskAssignedToSectionDomainEvent>
{
    public async Task Handle(TaskAssignedToSectionDomainEvent notification, CancellationToken ct)
    {
        await eventBus.PublishAsync<TaskAssignedToSectionEvent>(
            new(notification.TaskId, notification.SectionId, notification.CourseId, timeProvider.GetUtcNow()), ct);
    }
}
