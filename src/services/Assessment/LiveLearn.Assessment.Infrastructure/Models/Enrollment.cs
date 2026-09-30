namespace LiveLearn.Assessment.Infrastructure.Models;


public sealed class Enrollment
{

    public Guid StudentId { get; set; }

    public Guid CourseId { get; set; }

    public EnrollmentStatus Status { get; set; }

    public long Version { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}

public enum EnrollmentStatus
{
    Active,
    Revoked
}
