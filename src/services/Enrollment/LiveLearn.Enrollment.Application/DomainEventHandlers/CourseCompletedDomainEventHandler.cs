using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Enrollment;
using LiveLearn.Enrollment.Domain.DomainEvents;

namespace LiveLearn.Enrollment.Application.DomainEventHandlers;


internal sealed class CourseCompletedDomainEventHandler(IEventBus eventBus, TimeProvider timeProvider) :
    IDomainEventHandler<CourseCompletedDomainEvent>
{
    public async Task Handle(CourseCompletedDomainEvent notification, CancellationToken ct)
    {
        await eventBus.PublishAsync(new CourseCompletedEvent(
            notification.StudentId,
            notification.CourseId,
            timeProvider.GetUtcNow()), ct);
    }
}
