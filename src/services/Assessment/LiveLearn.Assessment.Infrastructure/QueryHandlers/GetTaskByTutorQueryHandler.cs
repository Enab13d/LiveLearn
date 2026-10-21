using LiveLearn.Assessment.Application.Dto;
using LiveLearn.Assessment.Application.Queries;
using LiveLearn.Assessment.Application.Services;
using LiveLearn.Assessment.Infrastructure.Contexts;
using LiveLearn.BuildingBlocks;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.QueryHandlers;


internal sealed class GetTasksByTutorQueryHandler(ICurrentUser currentUser, ReadDbContext dbContext) : IQueryHandler<GetTasksByTutorQuery, PagedResult<TaskSummaryDto>>
{
    public async Task<Result<PagedResult<TaskSummaryDto>>> Handle(GetTasksByTutorQuery request, CancellationToken ct)
    {
        var query = dbContext.AssessmentTasks
            .AsQueryable()
            .Where(e => e.TutorId == currentUser.UserId);

        if (!request.IncludeArchived)
            query = query.Where(e => e.ArchivedAt == null);

        int totalCount = await query.CountAsync(ct);

        var taskSummaries = await query
            .OrderBy(e => e.Title)
            .ThenBy(e => e.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new TaskSummaryDto(e.Id, e.Title, e.TaskType, e.ArchivedAt))
            .ToListAsync(ct);

        return new PagedResult<TaskSummaryDto>(taskSummaries, totalCount, request.PageNumber, request.PageSize);
    }
}
