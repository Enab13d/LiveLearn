namespace LiveLearn.Enrollment.Infrastructure.Models;

internal sealed class EnrollmentItemReadModel
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public Guid ItemId { get; set; }
    public string Kind { get; set; } = string.Empty;
    public DateTimeOffset? CompletedAt { get; set; }
}
