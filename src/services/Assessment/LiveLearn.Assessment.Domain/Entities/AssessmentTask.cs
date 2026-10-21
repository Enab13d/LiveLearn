using LiveLearn.Assessment.Domain.DomainEvents;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Domain.Entities;


public abstract class AssessmentTask : AggregateRoot<Guid>
{

    protected AssessmentTask() { }
    public string Title { get; protected set; } = string.Empty;
    public Guid TutorId { get; protected set; }
    public DateTimeOffset? ArchivedAt { get; protected set; }

    // Archived tasks are hidden from the assignment picker but stay submittable; a task is never deleted.
    public void Archive(DateTimeOffset at)
    {
        if (ArchivedAt is not null) return;

        ArchivedAt = at;
        RaiseDomainEvent(new TaskArchivedDomainEvent(Id));
    }
}
