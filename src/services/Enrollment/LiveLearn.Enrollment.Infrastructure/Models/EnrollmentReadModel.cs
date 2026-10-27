namespace LiveLearn.Enrollment.Infrastructure.Models;

internal sealed class EnrollmentReadModel
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string Status { get; set; } = string.Empty;
    public long CourseRevision { get; set; }
    public DateTimeOffset EnrolledAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public List<EnrollmentItemReadModel> Items { get; set; } = [];
}
