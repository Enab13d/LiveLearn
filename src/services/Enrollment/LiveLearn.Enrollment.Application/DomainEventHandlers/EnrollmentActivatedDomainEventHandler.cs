using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Enrollment;
using LiveLearn.Enrollment.Domain.DomainEvents;

namespace LiveLearn.Enrollment.Application.DomainEventHandlers;


// A new enrollment and a reactivated one both grant access, so both map to EnrollmentActivatedEvent.
internal sealed class EnrollmentActivatedDomainEventHandler(IEventBus eventBus, TimeProvider timeProvider) :
    IDomainEventHandler<EnrollmentCreatedDomainEvent>,
    IDomainEventHandler<EnrollmentActivatedDomainEvent>
{
    public Task Handle(EnrollmentCreatedDomainEvent notification, CancellationToken ct) =>
        PublishAsync(notification.EnrollmentId, notification.StudentId, notification.CourseId,
            notification.Version, notification.CourseRevision, notification.OrderId, notification.OrderLineId, ct);

    public Task Handle(EnrollmentActivatedDomainEvent notification, CancellationToken ct) =>
        PublishAsync(notification.EnrollmentId, notification.StudentId, notification.CourseId,
            notification.Version, notification.CourseRevision, notification.OrderId, notification.OrderLineId, ct);

    private Task PublishAsync(
        Guid enrollmentId, Guid studentId, Guid courseId, long version, long courseRevision,
        Guid? orderId, Guid? orderLineId, CancellationToken ct) =>
        eventBus.PublishAsync(new EnrollmentActivatedEvent(
            enrollmentId,
            studentId,
            courseId,
            version,
            courseRevision,
            timeProvider.GetUtcNow(),
            orderId,
            orderLineId), ct);
}
