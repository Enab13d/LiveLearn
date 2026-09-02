using LiveLearn.Assessment.Application.Dto;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Queries;


public sealed record GetTasksByTutorQuery(
    Guid TutorId,
    int PageNumber,
    int PageSize
) : IQuery<PagedResult<TaskSummaryDto>>;
