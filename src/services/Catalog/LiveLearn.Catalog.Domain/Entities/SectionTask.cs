using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.Entities;


public sealed class SectionTask : Entity<Guid>
{

    public Guid SectionId { get; private set; }

    public Guid CourseId { get; private set; }
    public Guid TaskId { get; private set; }

    public string TaskType { get; private set; } = string.Empty;
    public DateTimeOffset AssignedAt { get; private set; }
    public long CreatedRevision { get; private set; }
    public long? ArchivedRevision { get; private set; } = null;
    private SectionTask() { }
    internal SectionTask(Guid sectionId, Guid courseId, Guid taskId, string taskType, long createdRevision)
    {
        CreatedRevision = createdRevision;
        SectionId = sectionId;
        CourseId = courseId;
        TaskId = taskId;
        TaskType = taskType;
    }
    internal bool IsActive() => ArchivedRevision is null;

    internal bool IsVisibleAt(long revision) =>
        CreatedRevision <= revision && (ArchivedRevision is null || ArchivedRevision > revision);

    internal void Archive(long revision)
    {
        ArchivedRevision ??= revision;
    }

}
