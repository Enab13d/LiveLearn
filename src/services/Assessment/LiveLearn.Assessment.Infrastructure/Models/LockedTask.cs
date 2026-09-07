namespace LiveLearn.Assessment.Infrastructure.Models;


public sealed class LockedTask
{
    public Guid TaskId { get; set; }

    public Guid CourseId { get; set; }
}
