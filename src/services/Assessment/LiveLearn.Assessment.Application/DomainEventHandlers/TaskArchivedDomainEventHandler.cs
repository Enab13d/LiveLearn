using LiveLearn.Assessment.Domain.DomainEvents;
using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Assessment;

namespace LiveLearn.Assessment.Application.DomainEventHandlers;


internal sealed class TaskArchivedDomainEventHandler(
    IEventBus eventBus,
    TimeProvider timeProvider
) : IDomainEventHandler<TaskArchivedDomainEvent>
{
    public async Task Handle(TaskArchivedDomainEvent notification, CancellationToken ct)
    {
        await eventBus.PublishAsync<TaskArchivedEvent>(new(notification.TaskId, timeProvider.GetUtcNow()), ct);
    }
}
