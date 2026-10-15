using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Queries;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Domain.Errors;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Queries.Handlers;


internal sealed class GetLectureContentQueryHandler(ReadDbContext dbContext, ICourseContentAccess contentAccess) : IQueryHandler<GetLectureContentQuery, LectureContentDto>
{
    public async Task<Result<LectureContentDto>> Handle(GetLectureContentQuery request, CancellationToken ct)
    {
        var lecture = await dbContext.Courses
            .Where(c => c.Id == request.CourseId)
            .SelectMany(c => c.Sections
                .Where(s => s.Id == request.SectionId)
                .SelectMany(s => s.Lectures)
                .Where(l => l.Id == request.LectureId)
                .Select(l => new
                {
                    c.TutorId,
                    Content = new LectureContentDto(l.Id, l.Title, l.Description, l.Type, l.Order, l.DurationInSeconds, l.VideoId)
                }))
            .FirstOrDefaultAsync(ct);

        if (lecture is null)
            return Result<LectureContentDto>.Failure(LectureErrors.NotFound);

        return await contentAccess.CanAccessAsync(request.UserId, request.IsAdmin, lecture.TutorId, request.CourseId, ct)
            ? Result<LectureContentDto>.Success(lecture.Content)
            : Result<LectureContentDto>.Failure(LectureErrors.Forbidden);
    }
}
