namespace LiveLearn.Assessment.Infrastructure.Models;


// Insert-only, one row per published course the task belongs to; rows are never deleted.
public sealed class LockedTask
{
    public Guid TaskId { get; set; }

    public Guid CourseId { get; set; }
}
