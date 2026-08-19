using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.Entities;


public sealed class SectionTask : Entity<Guid>
{

    public Guid SectionId { get; private set; }

    public Guid CourseId { get; private set; }
    public Guid TaskId { get; private set; }

    public string TaskType { get; private set; } = string.Empty;
    public DateTimeOffset AssignedAt { get; private set; }
    private SectionTask() { }
    internal SectionTask(Guid sectionId, Guid courseId, Guid taskId, string taskType)
    {

        SectionId = sectionId;
        CourseId = courseId;
        TaskId = taskId;
        TaskType = taskType;
    }

}
