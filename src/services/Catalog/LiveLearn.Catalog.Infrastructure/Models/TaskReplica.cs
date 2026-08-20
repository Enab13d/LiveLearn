namespace LiveLearn.Catalog.Infrastructure.Models;


internal sealed class TaskReplica
{
    public Guid TaskId { get; set; }

    public Guid TutorId { get; set; }

    public string TaskType { get; set; } = string.Empty;
}
