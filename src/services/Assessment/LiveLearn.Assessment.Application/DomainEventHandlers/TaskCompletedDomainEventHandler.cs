using LiveLearn.Assessment.Domain.DomainEvents;
using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Assessment;

namespace LiveLearn.Assessment.Application.DomainEventHandlers;


internal sealed class TaskCompletedDomainEventHandler(
    IEventBus eventBus,
    TimeProvider timeProvider
) : IDomainEventHandler<TaskCompletedDomainEvent>
{
    public async Task Handle(TaskCompletedDomainEvent notification, CancellationToken ct)
    {
        var (taskId, studentId, sectionId, courseId) = notification;
        await eventBus.PublishAsync<TaskCompletedEvent>(
            new(taskId, studentId, sectionId, courseId, timeProvider.GetUtcNow()), ct);
    }
}
