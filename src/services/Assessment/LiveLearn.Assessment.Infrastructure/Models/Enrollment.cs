namespace LiveLearn.Assessment.Infrastructure.Models;


public sealed class Enrollment
{
    public Guid StudentId { get; set; }

    public Guid CourseId { get; set; }
}
