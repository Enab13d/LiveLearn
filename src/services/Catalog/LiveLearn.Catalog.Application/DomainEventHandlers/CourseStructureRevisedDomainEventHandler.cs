using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.DomainEvents;
using LiveLearn.Contracts.Catalog;

namespace LiveLearn.Catalog.Application.DomainEventHandlers;


// Maps every structural domain event (one per Course.Revision) to one CourseStructureRevisedEvent.
internal sealed class CourseStructureRevisedDomainEventHandler(IEventBus eventBus, TimeProvider timeProvider) :
    IDomainEventHandler<SectionAddedDomainEvent>,
    IDomainEventHandler<SectionArchivedDomainEvent>,
    IDomainEventHandler<LectureAddedDomainEvent>,
    IDomainEventHandler<LectureArchivedDomainEvent>,
    IDomainEventHandler<TaskAssignedToSectionDomainEvent>,
    IDomainEventHandler<TaskUnassignedFromSectionDomainEvent>
{
    public Task Handle(SectionAddedDomainEvent notification, CancellationToken ct) =>
        PublishAsync(notification.CourseId, notification.Revision, ct: ct);

    public Task Handle(SectionArchivedDomainEvent notification, CancellationToken ct) =>
        PublishAsync(notification.CourseId, notification.Revision,
            archivedLectureIds: notification.ArchivedLectureIds, archivedTaskIds: notification.ArchivedTaskIds, ct: ct);

    public Task Handle(LectureAddedDomainEvent notification, CancellationToken ct) =>
        PublishAsync(notification.CourseId, notification.Revision, addedLectureIds: [notification.LectureId], ct: ct);

    public Task Handle(LectureArchivedDomainEvent notification, CancellationToken ct) =>
        PublishAsync(notification.CourseId, notification.Revision, archivedLectureIds: [notification.LectureId], ct: ct);

    public Task Handle(TaskAssignedToSectionDomainEvent notification, CancellationToken ct) =>
        PublishAsync(notification.CourseId, notification.Revision, addedTaskIds: [notification.TaskId], ct: ct);

    public Task Handle(TaskUnassignedFromSectionDomainEvent notification, CancellationToken ct) =>
        PublishAsync(notification.CourseId, notification.Revision, archivedTaskIds: [notification.TaskId], ct: ct);

    private Task PublishAsync(
        Guid courseId,
        long revision,
        IReadOnlyList<Guid>? addedLectureIds = null,
        IReadOnlyList<Guid>? archivedLectureIds = null,
        IReadOnlyList<Guid>? addedTaskIds = null,
        IReadOnlyList<Guid>? archivedTaskIds = null,
        CancellationToken ct = default) =>
        eventBus.PublishAsync(new CourseStructureRevisedEvent(
            courseId,
            revision,
            addedLectureIds ?? [],
            archivedLectureIds ?? [],
            addedTaskIds ?? [],
            archivedTaskIds ?? [],
            timeProvider.GetUtcNow()), ct);
}
