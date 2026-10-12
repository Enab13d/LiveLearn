using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Domain.DomainEvents;
using LiveLearn.Contracts.Catalog;

namespace LiveLearn.Catalog.Application.DomainEventHandlers;


internal sealed class LectureVideoUploadedDomainEventHandler(IEventBus eventBus, TimeProvider timeProvider) : IDomainEventHandler<LectureVideoUploadedDomainEvent>
{
    public async Task Handle(LectureVideoUploadedDomainEvent notification, CancellationToken ct)
    {
        var message = new LectureVideoUploadedEvent(
            notification.CourseId,
            notification.LectureId,
            notification.VideoId,
            LectureVideoObjectKey.For(notification.CourseId, notification.LectureId, notification.VideoId),
            timeProvider.GetUtcNow());

        await eventBus.PublishAsync(message, ct);
    }
}
