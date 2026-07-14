using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Queries;
using LiveLearn.Catalog.Domain.Errors;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Queries.Handlers;

internal sealed class GetCourseByIdQueryHandler(ReadDbContext dbContext) : IQueryHandler<GetCourseByIdQuery, CourseDetailDto>
{
    public async Task<Result<CourseDetailDto>> Handle(GetCourseByIdQuery request, CancellationToken ct)
    {

        var courseDetails = await dbContext.Courses
        .Where(e => e.Id == request.CourseId)
        .Select(e => new CourseDetailDto(e.Id,
            e.CategoryId,
            e.Category.Name,
            e.TutorId,
            e.Title,
            e.Description,
            e.ThumbnailUrl,
            e.Price,
            e.Status,
            e.Sections
            .Select(e => new SectionDto(
                e.Id,
                e.Title,
                e.Order,
                e.Lectures
                .Select(e => new LectureDto(
                    e.Id, e.Title, e.Type, e.Order, e.DurationInSeconds))
                .ToList()
                ))
            .ToList()
            )
        ).FirstOrDefaultAsync(ct);

        if (courseDetails is null) return Result<CourseDetailDto>.Failure(CourseErrors.NotFound);
        return courseDetails;


    }
}
