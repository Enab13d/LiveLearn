using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Queries;
using LiveLearn.Catalog.Domain.Enums;
using LiveLearn.Catalog.Domain.Errors;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Queries.Handlers;

internal sealed class GetCourseByIdQueryHandler(ReadDbContext dbContext) : IQueryHandler<GetCourseByIdQuery, CourseDetailDto>
{
    public async Task<Result<CourseDetailDto>> Handle(GetCourseByIdQuery request, CancellationToken ct)
    {
        var courseDetails = await dbContext.Courses
        .Where(e => e.Id == request.CourseId && (e.Status == CourseStatus.Published.ToString() || e.TutorId == request.RequesterId))
        .Select(c => new CourseDetailDto(c.Id,
            c.CategoryId,
            c.Category.Name,
            c.TutorId,
            c.Title,
            c.Description,
            c.ThumbnailUrl,
            c.Price,
            c.Status,
            c.Sections
            .Where(s => s.ArchivedRevision == null || c.TutorId == request.RequesterId)
            .OrderBy(s => s.Order)
            .Select(s => new SectionDto(
                s.Id,
                s.Title,
                s.Order,
                s.Lectures
                .Where(l => l.ArchivedRevision == null || c.TutorId == request.RequesterId)
                .OrderBy(l => l.Order)
                .Select(l => new LectureDto(
                    l.Id, l.Title, l.Type, l.Order, l.DurationInSeconds, l.ArchivedRevision != null))
                .ToList(),
                s.SectionTasks
                .Where(t => t.ArchivedRevision == null || c.TutorId == request.RequesterId)
                .Select(t => new SectionTaskDto(
                    t.SectionId, t.CourseId, t.TaskId, t.TaskType, t.ArchivedRevision != null))
                .ToList(),
                s.ArchivedRevision != null
                ))
            .ToList()
            )
        )
        .AsSplitQuery()
        .FirstOrDefaultAsync(ct);

        if (courseDetails is null) return Result<CourseDetailDto>.Failure(CourseErrors.NotFound);
        return courseDetails;
    }
}
