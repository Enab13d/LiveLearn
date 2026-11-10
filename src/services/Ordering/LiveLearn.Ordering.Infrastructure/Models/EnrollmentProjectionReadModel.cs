namespace LiveLearn.Ordering.Infrastructure.Models;


internal sealed class EnrollmentProjectionReadModel
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public bool IsActive { get; set; }
}
