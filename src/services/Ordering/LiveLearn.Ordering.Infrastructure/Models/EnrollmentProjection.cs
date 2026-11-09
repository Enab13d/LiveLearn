namespace LiveLearn.Ordering.Infrastructure.Models;


// Local copy of a student's access to a course (O9). Enrollment events arrive in any order, so an event is
// applied only if its Version is newer than the stored one — a late Activated never resurrects a Revoked row.
public sealed class EnrollmentProjection
{
    public Guid StudentId { get; private set; }

    public Guid CourseId { get; private set; }

    public bool IsActive { get; private set; }

    // Enrollment aggregate Version of the last applied event.
    public long Version { get; private set; }

    private EnrollmentProjection() { }

    public static EnrollmentProjection Create(Guid studentId, Guid courseId, bool isActive, long version) =>
        new() { StudentId = studentId, CourseId = courseId, IsActive = isActive, Version = version };

    // Returns false when the event is a duplicate or older than the stored state, so nothing needs saving.
    public bool Apply(bool isActive, long version)
    {
        if (version <= Version) return false;

        IsActive = isActive;
        Version = version;
        return true;
    }
}
