using LiveLearn.Ordering.Domain.Enums;

namespace LiveLearn.Ordering.Infrastructure.Models;


internal sealed class CourseProjectionReadModel
{
    public Guid CourseId { get; set; }
    public Guid? OwnerId { get; set; }
    public decimal? Price { get; set; }
    public CourseStatus Status { get; set; }
}
