using LiveLearn.Catalog.Domain.Enums;

namespace LiveLearn.Catalog.Domain.Common;

// How a user may see a course's structure. Revision is set only for EnrolledRevision.
public sealed record CourseAccessContext
{
    public static readonly CourseAccessContext None = new(CourseAccessMode.None, null);
    public static readonly CourseAccessContext CurrentRevision = new(CourseAccessMode.CurrentRevision, null);
    public static readonly CourseAccessContext Full = new(CourseAccessMode.Full, null);

    public CourseAccessMode Mode { get; }

    public long? Revision { get; }

    private CourseAccessContext(CourseAccessMode mode, long? revision)
    {
        Mode = mode;
        Revision = revision;
    }

    public static CourseAccessContext EnrolledRevision(long revision) => new(CourseAccessMode.EnrolledRevision, revision);
}
