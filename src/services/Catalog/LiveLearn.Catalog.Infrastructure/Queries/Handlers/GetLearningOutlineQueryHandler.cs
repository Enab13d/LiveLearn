using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Queries;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Domain.Enums;
using LiveLearn.Catalog.Domain.Errors;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Queries.Handlers;


internal sealed class GetLearningOutlineQueryHandler(ICourseAccess courseAccess, ReadDbContext dbContext) : IQueryHandler<GetLearningOutlineQuery, CourseDetailDto>
{
    public async Task<Result<CourseDetailDto>> Handle(GetLearningOutlineQuery request, CancellationToken ct)
    {
        //this query assume that only enrolled user can see the course outline in specific revision regardless of the course status
        var access = await courseAccess.GetLearningAccessAsync(request.CourseId, ct);
        if (access is null) return Result<CourseDetailDto>.Failure(CourseErrors.NotFound);
        if (access.Mode != CourseAccessMode.EnrolledRevision) return Result<CourseDetailDto>.Failure(CourseErrors.NotEnrolled);

        var courseDetail = await dbContext.Courses
            .Where(c => c.Id == request.CourseId)
            .Select(CourseProjections.ToCourseDetail(access))
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct);

        return courseDetail is null
            ? Result<CourseDetailDto>.Failure(CourseErrors.NotFound)
            : Result<CourseDetailDto>.Success(courseDetail);
    }
}
