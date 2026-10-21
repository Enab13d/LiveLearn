using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Queries;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Queries.Handlers;


internal sealed class GetMyCoursesQueryHandler(ICurrentUser currentUser, ReadDbContext dbContext) : IQueryHandler<GetMyCoursesQuery, PagedResult<CourseDto>>
{
    public async Task<Result<PagedResult<CourseDto>>> Handle(GetMyCoursesQuery request, CancellationToken ct)
    {
        var query = dbContext.Courses.Where(c => c.TutorId == currentUser.UserId);

        int totalCount = await query.CountAsync(ct);

        var courses = await query
            .OrderBy(c => c.Title)
            .ThenBy(c => c.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CourseDto(c.Id, c.TutorId, c.CategoryId, c.Title, c.Description, c.ThumbnailUrl, c.Price, c.Status))
            .ToListAsync(ct);

        return new PagedResult<CourseDto>(courses, totalCount, request.PageNumber, request.PageSize);
    }
}
