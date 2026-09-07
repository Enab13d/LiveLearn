using LiveLearn.Assessment.Domain.DomainEvents;
using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Assessment;

namespace LiveLearn.Assessment.Application.DomainEventHandlers;


internal sealed class TaskCreatedDomainEventHandler(IEventBus eventBus, TimeProvider timeProvider) : IDomainEventHandler<TaskCreatedDomainEvent>
{
    public async Task Handle(TaskCreatedDomainEvent notification, CancellationToken ct)
    {
        var (taskId, tutorId, taskType) = notification;
        await eventBus.PublishAsync<TaskCreatedEvent>(
            new(taskId, tutorId, taskType, timeProvider.GetUtcNow()), ct);
    }
}
