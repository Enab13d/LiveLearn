using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Queries;
using LiveLearn.Catalog.Domain.Errors;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Queries.Handlers;


internal sealed class GetLectureVideoStatusQueryHandler(ReadDbContext dbContext) : IQueryHandler<GetLectureVideoStatusQuery, string>
{
    public async Task<Result<string>> Handle(GetLectureVideoStatusQuery request, CancellationToken ct)
    {
        var status = await dbContext.Courses
            .Where(e => e.Id == request.CourseId && e.TutorId == request.TutorId)
            .SelectMany(e => e.Sections)
            .SelectMany(e => e.Lectures)
            .Where(l => l.Id == request.LectureId)
            .Select(l => l.VideoStatus)
            .FirstOrDefaultAsync(ct);

        return status is null ? Result<string>.Failure(LectureErrors.NotFound) : Result<string>.Success(status);
    }
}
