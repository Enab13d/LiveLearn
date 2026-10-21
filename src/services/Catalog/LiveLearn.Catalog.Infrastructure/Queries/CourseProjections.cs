using System.Linq.Expressions;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Domain.Common;
using LiveLearn.Catalog.Infrastructure.Models;

namespace LiveLearn.Catalog.Infrastructure.Queries;


// Course structure projections filtered by what the access context may see.
internal static class CourseProjections
{
    public static Expression<Func<CourseReadModel, CourseDetailDto>> ToCourseDetail(CourseAccessContext access)
    {
        var sectionVisible = SectionReadModel.VisibleTo(access);
        var toSection = ToSection(access);

        return c => new CourseDetailDto(
            c.Id,
            c.CategoryId,
            c.Category.Name,
            c.TutorId,
            c.Title,
            c.Description,
            c.ThumbnailUrl,
            c.Price,
            c.Status,
            c.Sections.AsQueryable()
                .Where(sectionVisible)
                .OrderBy(s => s.Order)
                .Select(toSection)
                .ToList());
    }

    public static Expression<Func<SectionReadModel, SectionDto>> ToSection(CourseAccessContext access)
    {
        var lectureVisible = LectureReadModel.VisibleTo(access);
        var taskVisible = SectionTaskReadModel.VisibleTo(access);

        return s => new SectionDto(
            s.Id,
            s.Title,
            s.Order,
            s.Lectures.AsQueryable()
                .Where(lectureVisible)
                .OrderBy(l => l.Order)
                .Select(l => new LectureDto(l.Id, l.Title, l.Type, l.Order, l.DurationInSeconds, l.ArchivedRevision != null))
                .ToList(),
            s.SectionTasks.AsQueryable()
                .Where(taskVisible)
                .Select(t => new SectionTaskDto(t.SectionId, t.CourseId, t.TaskId, t.TaskType, t.ArchivedRevision != null))
                .ToList(),
            s.ArchivedRevision != null);
    }
}
