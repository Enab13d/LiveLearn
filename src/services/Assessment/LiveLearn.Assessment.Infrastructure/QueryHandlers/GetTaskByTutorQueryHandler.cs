using LiveLearn.Assessment.Application.Dto;
using LiveLearn.Assessment.Application.Queries;
using LiveLearn.Assessment.Infrastructure.Contexts;
using LiveLearn.BuildingBlocks;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.QueryHandlers;


internal sealed class GetTasksByTutorQueryHandler(ReadDbContext dbContext) : IQueryHandler<GetTasksByTutorQuery, PagedResult<TaskSummaryDto>>
{
    public async Task<Result<PagedResult<TaskSummaryDto>>> Handle(GetTasksByTutorQuery request, CancellationToken ct)
    {
        var query = dbContext.AssessmentTasks
            .AsQueryable()
            .Where(e => e.TutorId == request.TutorId);

        if (request.Assigned)
            query = query.Where(e => e.CourseId != null && e.SectionId != null);

        int totalCount = await query.CountAsync(ct);

        var taskSummaries = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new TaskSummaryDto(e.Id, e.Title, e.TaskType, e.SectionId, e.CourseId))
            .ToListAsync(ct);

        return new PagedResult<TaskSummaryDto>(taskSummaries, totalCount, request.PageNumber, request.PageSize);
    }
}
