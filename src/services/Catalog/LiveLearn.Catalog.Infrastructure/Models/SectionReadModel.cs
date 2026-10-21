using System.Linq.Expressions;
using LiveLearn.Catalog.Domain.Common;
using LiveLearn.Catalog.Domain.Enums;

namespace LiveLearn.Catalog.Infrastructure.Models;


internal class SectionReadModel
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public long CreatedRevision { get; set; }
    public long? ArchivedRevision { get; set; }
    public List<LectureReadModel> Lectures { get; set; } = [];

    public List<SectionTaskReadModel> SectionTasks { get; set; } = [];

    public static readonly Expression<Func<SectionReadModel, bool>> IsActive = s => s.ArchivedRevision == null;

    // A student sees a section iff it contains at least one item visible at their revision — so a lecture
    // moved into a section created after the snapshot stays visible.
    public static Expression<Func<SectionReadModel, bool>> VisibleTo(CourseAccessContext access)
    {
        switch (access)
        {
            case { Mode: CourseAccessMode.Full }:
                return _ => true;
            case { Mode: CourseAccessMode.EnrolledRevision, Revision: long revision }:
                var lectureVisible = LectureReadModel.IsVisibleAt(revision);
                var taskVisible = SectionTaskReadModel.IsVisibleAt(revision);
                return s => s.Lectures.AsQueryable().Any(lectureVisible) || s.SectionTasks.AsQueryable().Any(taskVisible);
            case { Mode: CourseAccessMode.CurrentRevision }:
                return IsActive;
            default:
                return _ => false;
        }
    }
}
