using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.DomainEvents;
using LiveLearn.Contracts.Catalog;

namespace LiveLearn.Catalog.Application.DomainEventHandlers;


internal sealed class CourseCreatedDomainEventHandler(IEventBus eventBus, TimeProvider timeProvider) : IDomainEventHandler<CourseCreatedDomainEvent>
{
    public async Task Handle(CourseCreatedDomainEvent notification, CancellationToken ct)
    {
        var message = new CourseCreatedEvent(
            notification.Id, notification.TutorId, timeProvider.GetUtcNow());
        
        await eventBus.PublishAsync(message, ct);
    }
}
