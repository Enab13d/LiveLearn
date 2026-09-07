using LiveLearn.Assessment.Domain.DomainEvents;
using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Assessment;

namespace LiveLearn.Assessment.Application.DomainEventHandlers;


internal sealed class TaskDeletedDomainEventHandler(
    IEventBus eventBus,
    TimeProvider timeProvider
) : IDomainEventHandler<TaskDeletedDomainEvent>
{
    public async Task Handle(TaskDeletedDomainEvent notification, CancellationToken ct)
    {
        await eventBus.PublishAsync<TaskDeletedEvent>(new(notification.TaskId, timeProvider.GetUtcNow()), ct);
    }
}
