using LiveLearn.Assessment.Domain.DomainEvents;
using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Assessment;

namespace LiveLearn.Assessment.Application.DomainEventHandlers;


internal sealed class TaskFailedDomainEventHandler(
    IEventBus eventBus,
    TimeProvider timeProvider
) : IDomainEventHandler<TaskFailedDomainEvent>
{
    public async Task Handle(TaskFailedDomainEvent notification, CancellationToken ct)
    {
        var (taskId, studentId, sectionId, courseId) = notification;
        await eventBus.PublishAsync<TaskFailedEvent>(
            new(studentId, taskId, sectionId, courseId, timeProvider.GetUtcNow()), ct);
    }
}
