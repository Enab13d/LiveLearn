using System.Linq.Expressions;
using LiveLearn.Catalog.Domain.Common;
using LiveLearn.Catalog.Domain.Enums;

namespace LiveLearn.Catalog.Infrastructure.Models;

internal class LectureReadModel
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public int Order { get; set; }

    public int? DurationInSeconds { get; set; }

    public string Description { get; set; } = string.Empty;

    public Guid? VideoId { get; set; }

    public string VideoStatus { get; set; } = string.Empty;

    public long CreatedRevision { get; set; }

    public long? ArchivedRevision { get; set; }

    // Expression counterparts of the domain's IsActive / IsVisibleAt, so EF Core can translate them.
    public static readonly Expression<Func<LectureReadModel, bool>> IsActive = e => e.ArchivedRevision == null;

    public static Expression<Func<LectureReadModel, bool>> IsVisibleAt(long revision) =>
        e => e.CreatedRevision <= revision && (e.ArchivedRevision == null || e.ArchivedRevision > revision);

    public static Expression<Func<LectureReadModel, bool>> VisibleTo(CourseAccessContext access) => access switch
    {
        { Mode: CourseAccessMode.Full } => _ => true,
        { Mode: CourseAccessMode.EnrolledRevision, Revision: long revision } => IsVisibleAt(revision),
        { Mode: CourseAccessMode.CurrentRevision } => IsActive,
        _ => _ => false
    };
}
