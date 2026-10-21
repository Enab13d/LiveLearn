using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Domain.Common;
using LiveLearn.Enrollment.Domain.DomainEvents;
using LiveLearn.Enrollment.Domain.Enums;
using LiveLearn.Enrollment.Domain.Errors;

namespace LiveLearn.Enrollment.Domain.Entities;


public sealed class CourseEnrollment : AggregateRoot<Guid>
{
    public Guid StudentId { get; private set; }

    public Guid CourseId { get; private set; }

    public EnrollmentStatus Status { get; private set; }

    public RevocationReason? RevocationReason { get; private set; } = null;

    public long Version { get; private set; }

    public long CourseRevision { get; private set; }

    public DateTimeOffset EnrolledAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    private readonly List<EnrollmentItem> _items = [];

    public IReadOnlyCollection<EnrollmentItem> Items => _items.AsReadOnly();

    private CourseEnrollment() { }


    public static CourseEnrollment Create(Guid id, Guid studentId, Guid courseId, CourseSnapshot snapshot, DateTimeOffset enrolledAt)
    {
        CourseEnrollment enrollment = new()
        {
            Id = id,
            CourseId = courseId,
            StudentId = studentId,
            EnrolledAt = enrolledAt,
            Status = EnrollmentStatus.Active,
            CourseRevision = snapshot.Revision,
        };

        enrollment._items.AddRange(enrollment.BuildItems(snapshot, new Dictionary<Guid, DateTimeOffset>()));

        enrollment.RaiseDomainEvent(new EnrollmentCreatedDomainEvent(id, studentId, courseId, enrollment.Version, enrollment.CourseRevision));

        return enrollment;
    }

    // Items outside the snapshot and already completed items are a no-op, so redelivered events are harmless.
    public Result CompleteItem(Guid itemId, EnrollmentItemKind kind, DateTimeOffset completedAt)
    {
        if (Status == EnrollmentStatus.Revoked) return Result.Failure(EnrollmentErrors.EnrollmentRevoked);

        var item = _items.FirstOrDefault(e => e.ItemId == itemId && e.Kind == kind);
        if (item is null || item.IsCompleted) return Result.Success();

        item.Complete(completedAt);
        TryComplete(completedAt);

        return Result.Success();
    }

    public void Revoke(RevocationReason revocationReason)
    {

        if (Status == EnrollmentStatus.Revoked) return;

        Status = EnrollmentStatus.Revoked;
        RevocationReason = revocationReason;
        Version++;

        RaiseDomainEvent(new EnrollmentRevokedDomainEvent(Id, StudentId, CourseId, Version, CourseRevision, revocationReason.ToString()));


    }

    // Repurchase after refund: moves to the latest snapshot, carrying over progress by item ID.
    public void Reactivate(CourseSnapshot snapshot, DateTimeOffset reactivatedAt)
    {
        if (Status != EnrollmentStatus.Revoked) return;
        RevocationReason = null;
        Version++;
        CourseRevision = snapshot.Revision;

        var completedAtByItemId = _items
            .Where(e => e.IsCompleted)
            .ToDictionary(e => e.ItemId, e => e.CompletedAt!.Value);

        var items = BuildItems(snapshot, completedAtByItemId);
        _items.Clear();
        _items.AddRange(items);

        // A course completed once stays completed and never raises a second CourseCompletedDomainEvent.
        Status = CompletedAt is not null ? EnrollmentStatus.Completed : EnrollmentStatus.Active;

        RaiseDomainEvent(new EnrollmentActivatedDomainEvent(Id, StudentId, CourseId, Version, CourseRevision));

        TryComplete(reactivatedAt);
    }

    private void TryComplete(DateTimeOffset completedAt)
    {
        if (Status != EnrollmentStatus.Active || _items.Count == 0 || CompletedAt is not null) return;
        if (!_items.All(e => e.IsCompleted)) return;

        Status = EnrollmentStatus.Completed;
        CompletedAt = completedAt;
        RaiseDomainEvent(new CourseCompletedDomainEvent(Id, CourseId, StudentId));
    }

    private IEnumerable<EnrollmentItem> BuildItems(CourseSnapshot snapshot, IReadOnlyDictionary<Guid, DateTimeOffset> completedAtByItemId)
    {
        var lectures = snapshot.LectureIds.Select(lectureId => (ItemId: lectureId, Kind: EnrollmentItemKind.Lecture));
        var tasks = snapshot.TaskIds.Select(taskId => (ItemId: taskId, Kind: EnrollmentItemKind.Task));

        return lectures.Concat(tasks).Select(e => EnrollmentItem.Create(
            Id,
            e.ItemId,
            e.Kind,
            completedAtByItemId.TryGetValue(e.ItemId, out var completedAt) ? completedAt : null));
    }

}
