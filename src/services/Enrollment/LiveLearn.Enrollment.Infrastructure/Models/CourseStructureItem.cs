using LiveLearn.Enrollment.Domain.Enums;

namespace LiveLearn.Enrollment.Infrastructure.Models;

// A lecture or task link active at the course's projected revision; archiving deletes the row.
public sealed class CourseStructureItem
{
    public Guid CourseId { get; set; }
    public Guid ItemId { get; set; }
    public EnrollmentItemKind Kind { get; set; }
}
