using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Queries;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Domain.Enums;
using LiveLearn.Catalog.Domain.Errors;
using LiveLearn.Catalog.Infrastructure.Contexts;
using LiveLearn.Catalog.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Queries.Handlers;


internal sealed class GetLectureContentQueryHandler(ICourseAccess courseAccess, ReadDbContext dbContext) : IQueryHandler<GetLectureContentQuery, LectureContentDto>
{
    public async Task<Result<LectureContentDto>> Handle(GetLectureContentQuery request, CancellationToken ct)
    {
        //this query assume that authenticated enrolled user OR course owner OR admin can see the lecture content.
        // enrolled user can see the lecture content only if it's present in respective course revision.
        // Admin and course owner can see the lecture content anyway
        var access = await courseAccess.GetLearningAccessAsync(request.CourseId, ct);
        if (access is null) return Result<LectureContentDto>.Failure(CourseErrors.NotFound);
        if (access.Mode == CourseAccessMode.None) return Result<LectureContentDto>.Failure(LectureErrors.Forbidden);

        var lecture = await dbContext.Sections
            .Where(s => s.Id == request.SectionId && s.CourseId == request.CourseId)
            .SelectMany(s => s.Lectures)
            .Where(l => l.Id == request.LectureId)
            .Where(LectureReadModel.VisibleTo(access))
            .Select(l => new LectureContentDto(l.Id, l.Title, l.Description, l.Type, l.Order, l.DurationInSeconds, l.VideoId))
            .FirstOrDefaultAsync(ct);

        return lecture is null
            ? Result<LectureContentDto>.Failure(LectureErrors.NotFound)
            : Result<LectureContentDto>.Success(lecture);
    }
}
