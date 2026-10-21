namespace LiveLearn.Assessment.Infrastructure.Models;


// Insert-only: archiving a course never unlocks its tasks.
public sealed class PublishedCourse
{
    public Guid CourseId { get; set; }
}
