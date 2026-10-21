namespace LiveLearn.Assessment.Infrastructure.Models;


// Insert-only: the task was assigned to the course in some revision. Backs ICourseTaskLookup.
public sealed class CourseTask
{
    public Guid CourseId { get; set; }

    public Guid TaskId { get; set; }
}
