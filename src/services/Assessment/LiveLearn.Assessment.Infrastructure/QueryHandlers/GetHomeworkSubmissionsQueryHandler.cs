using LiveLearn.Assessment.Application.Dto;
using LiveLearn.Assessment.Application.Queries;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.Assessment.Infrastructure.Contexts;
using LiveLearn.BuildingBlocks;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.QueryHandlers;


internal sealed class GetHomeworkSubmissionsQueryHandler(ReadDbContext dbContext) : IQueryHandler<GetHomeworkSubmissionsQuery, PagedResult<HomeworkSubmissionDto>>
{
    public async Task<Result<PagedResult<HomeworkSubmissionDto>>> Handle(GetHomeworkSubmissionsQuery request, CancellationToken ct)
    {

        var homework = await dbContext.Homeworks.FindAsync([request.HomeworkId], ct);
        if (homework is null) return Result<PagedResult<HomeworkSubmissionDto>>.Failure(TaskErrors.NotFound);
        if (homework.TutorId != request.TutorId) return Result<PagedResult<HomeworkSubmissionDto>>.Failure(TaskErrors.Forbidden);

        var query = dbContext.HomeworkSubmissions
            .AsQueryable()
            .Where(e => e.HomeworkId == request.HomeworkId);

        if (request.StudentId is not null)
            query = query.Where(e => e.StudentId == request.StudentId);

        if (request.CourseId is not null)
            query = query.Where(e => e.CourseId == request.CourseId);

        if (request.SectionId is not null)
            query = query.Where(e => e.SectionId == request.SectionId);

        if (request.Status is not null)
            query = query.Where(e => e.Status == request.Status.Value.ToString());

        if (request.StartDate is not null)
            query = query.Where(e => e.SubmittedAt >= request.StartDate);

        if (request.EndDate is not null)
            query = query.Where(e => e.SubmittedAt <= request.EndDate);


        int totalCount = await query.CountAsync(ct);

        var homeworkSubmissions = await query
            .OrderByDescending(e => e.SubmittedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new HomeworkSubmissionDto(
                e.Id, e.StudentId, e.SectionId, e.CourseId, e.Content, e.Status, e.InstructorFeedback, e.SubmittedAt))
            .ToListAsync(ct);

        return new PagedResult<HomeworkSubmissionDto>(homeworkSubmissions, totalCount, request.PageNumber, request.PageSize);
    }
}
