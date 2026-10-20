using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Queries;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Domain.Errors;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Queries.Handlers;


internal sealed class GetLearningOutlineQueryHandler(IEnrollmentLookup enrollmentLookup, ReadDbContext dbContext) : IQueryHandler<GetLearningOutlineQuery, CourseDetailDto>
{
    public async Task<Result<CourseDetailDto>> Handle(GetLearningOutlineQuery request, CancellationToken ct)
    {
        var enrollment = await enrollmentLookup.GetActiveAsync(request.UserId, request.CourseId, ct);
        if (enrollment is null) return Result<CourseDetailDto>.Failure(CourseErrors.NotEnrolled);
        var rev = enrollment.CourseRevision;

        var courseDetail = await dbContext.Courses
            .Where(c => c.Id == request.CourseId)
            .Select(c => new CourseDetailDto(
                c.Id,
                c.CategoryId,
                c.Category.Name,
                c.TutorId,
                c.Title,
                c.Description,
                c.ThumbnailUrl,
                c.Price,
                c.Status,
                c.Sections
                    .Where(s => s.Lectures
                        .Any(l => l.CreatedRevision <= rev && (l.ArchivedRevision == null || l.ArchivedRevision > rev))
                             || s.SectionTasks.Any(t => t.CreatedRevision <= rev && (t.ArchivedRevision == null || t.ArchivedRevision > rev)))
                    .OrderBy(s => s.Order)
                    .Select(s => new SectionDto(
                        s.Id,
                        s.Title,
                        s.Order,
                        s.Lectures
                            .Where(l => l.CreatedRevision <= rev && (l.ArchivedRevision == null || l.ArchivedRevision > rev))
                            .OrderBy(l => l.Order)
                            .Select(l => 
                                new LectureDto(l.Id, l.Title, l.Type, l.Order, l.DurationInSeconds, IsArchived: l.ArchivedRevision != null))
                            .ToList(),
                        s.SectionTasks.Where(t => t.CreatedRevision <= rev && (t.ArchivedRevision == null || t.ArchivedRevision > rev))
                            .Select(t => 
                                new SectionTaskDto(t.SectionId, t.CourseId, t.TaskId, t.TaskType, IsArchived: t.ArchivedRevision != null))
                            .ToList(),
                        IsArchived: s.ArchivedRevision != null))
                        .ToList()
            ))
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct);

        return courseDetail is null
            ? Result<CourseDetailDto>.Failure(CourseErrors.NotFound)
            : Result<CourseDetailDto>.Success(courseDetail);
    }
}
