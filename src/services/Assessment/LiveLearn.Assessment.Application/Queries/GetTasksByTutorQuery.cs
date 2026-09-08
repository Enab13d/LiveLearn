using LiveLearn.Assessment.Application.Dto;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Queries;


public sealed record GetTasksByTutorQuery(
    Guid TutorId,
    bool Assigned = false,
    int PageNumber = 1,
    int PageSize = 10
) : IQuery<PagedResult<TaskSummaryDto>>;
