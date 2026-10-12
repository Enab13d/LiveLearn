namespace LiveLearn.Catalog.Infrastructure.Models;


// Local projection of Enrollment's state, populated only by the enrollment consumers.
public sealed class CourseEnrollment
{
    public Guid StudentId { get; set; }

    public Guid CourseId { get; set; }

    public CourseEnrollmentStatus Status { get; set; }

    public long CourseRevision { get; set; }

    public long Version { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}

public enum CourseEnrollmentStatus
{
    Active,
    Revoked
}
