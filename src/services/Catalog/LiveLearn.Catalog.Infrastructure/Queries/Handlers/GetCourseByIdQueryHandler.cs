using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Queries;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Domain.Enums;
using LiveLearn.Catalog.Domain.Errors;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Queries.Handlers;

internal sealed class GetCourseByIdQueryHandler(ICourseAccess courseAccess, ReadDbContext dbContext) : IQueryHandler<GetCourseByIdQuery, CourseDetailDto>
{
    public async Task<Result<CourseDetailDto>> Handle(GetCourseByIdQuery request, CancellationToken ct)
    {
        //this query assume each user (even unauthenticated) can see the course outline
        //Course owner and admin can read course regardless of it's status including archived content.
        // Other users see only if course is published AND only current revision content.
        var access = await courseAccess.GetBrowseAccessAsync(request.CourseId, ct);
        if (access is null || access.Mode == CourseAccessMode.None)
            return Result<CourseDetailDto>.Failure(CourseErrors.NotFound);

        var courseDetails = await dbContext.Courses
            .Where(c => c.Id == request.CourseId)
            .Select(CourseProjections.ToCourseDetail(access))
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct);

        if (courseDetails is null) return Result<CourseDetailDto>.Failure(CourseErrors.NotFound);
        return courseDetails;
    }
}
