namespace LiveLearn.Enrollment.Infrastructure.Models;

// Latest Catalog revision applied to the structure projection — revisions are applied strictly in
// order, so every revision 1..Revision is applied. Enrollments snapshot at Revision.
public sealed class CourseRevision
{
    public Guid CourseId { get; set; }
    public long Revision { get; set; }
}
