namespace LiveLearn.Catalog.Infrastructure.Models;


public sealed class TaskReplica
{
    internal TaskReplica(Guid taskId, Guid tutorId, string taskType)
    {
        TaskId = taskId;
        TutorId = tutorId;
        TaskType = taskType;
    }
    public Guid TaskId { get; set; }

    public Guid TutorId { get; set; }

    public string TaskType { get; set; } = string.Empty;
}
