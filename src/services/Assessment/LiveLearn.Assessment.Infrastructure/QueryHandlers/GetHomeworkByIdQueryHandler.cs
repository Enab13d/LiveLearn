using LiveLearn.Assessment.Application.Dto;
using LiveLearn.Assessment.Application.Queries;
using LiveLearn.Assessment.Application.Services;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.Assessment.Infrastructure.Contexts;
using LiveLearn.BuildingBlocks;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.QueryHandlers;


internal sealed class GetHomeworkByIdQueryHandler(
    ReadDbContext dbContext,
    IEnrollmentLookup enrollmentLookup,
    ICourseTaskLookup courseTaskLookup
) : IQueryHandler<GetHomeworkByIdQuery, HomeworkDto>
{
    public async Task<Result<HomeworkDto>> Handle(GetHomeworkByIdQuery request, CancellationToken ct)
    {
        var homework = await dbContext.Homeworks.FirstOrDefaultAsync(e => e.Id == request.Id, ct);
        if (homework is null) return Result<HomeworkDto>.Failure(TaskErrors.NotFound);

        // owner, or enrolled in a course the homework was ever assigned to
        if (request.RequesterId != homework.TutorId
            && (request.CourseId is not Guid courseId
                || !await enrollmentLookup.IsEnrolledAsync(request.RequesterId, courseId, ct)
                || !await courseTaskLookup.WasEverAssignedAsync(courseId, homework.Id, ct)))
            return Result<HomeworkDto>.Failure(TaskErrors.Forbidden);

        return new HomeworkDto(homework.Id, homework.TutorId, homework.Title, homework.Description);

    }
}
