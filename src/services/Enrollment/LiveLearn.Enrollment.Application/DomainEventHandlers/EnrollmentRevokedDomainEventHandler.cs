using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Enrollment;
using LiveLearn.Enrollment.Domain.DomainEvents;

namespace LiveLearn.Enrollment.Application.DomainEventHandlers;


internal sealed class EnrollmentRevokedDomainEventHandler(IEventBus eventBus, TimeProvider timeProvider) :
    IDomainEventHandler<EnrollmentRevokedDomainEvent>
{
    public async Task Handle(EnrollmentRevokedDomainEvent notification, CancellationToken ct)
    {
        await eventBus.PublishAsync(new EnrollmentRevokedEvent(
            notification.EnrollmentId,
            notification.StudentId,
            notification.CourseId,
            notification.Version,
            notification.CourseRevision,
            timeProvider.GetUtcNow(),
            notification.Reason), ct);
    }
}
