using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Queries;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Queries.Handlers;


internal sealed class GetCoursesByTutorQueryHandler(ReadDbContext dbContext)
: IQueryHandler<GetCoursesByTutorQuery, PagedResult<CourseDto>>
{
    public async Task<Result<PagedResult<CourseDto>>> Handle(GetCoursesByTutorQuery request, CancellationToken ct)
    {
        var query = dbContext.Courses
            .Where(c => c.TutorId == request.TutorId)
            .Select(c => new CourseDto(c.Id, c.TutorId, c.CategoryId, c.Title, c.Description, c.ThumbnailUrl, c.Price, c.Status));

        int totalCount = await query.CountAsync(ct);

        if (request.PageNumber > 0 && request.PageSize > 0)
            query = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var courses = await query.ToListAsync(ct);

        return new PagedResult<CourseDto>(courses, totalCount, request.PageNumber, request.PageSize);


    }
}
