namespace LiveLearn.Catalog.Domain.Enums;

public enum CourseAccessMode
{
    // No access to the course content.
    None,
    // Items active at the course's current Revision (published course, any visitor).
    CurrentRevision,
    // Items visible at the student's enrollment revision.
    EnrolledRevision,
    // Every item, archived included (course owner, admin).
    Full
}
