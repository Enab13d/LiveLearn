namespace LiveLearn.Assessment.Infrastructure.Models;


public sealed class Enrollment
{
    public Guid Id { get; set; }
    
    public Guid StudentId { get; set; }

    public Guid CourseId { get; set; }
}
