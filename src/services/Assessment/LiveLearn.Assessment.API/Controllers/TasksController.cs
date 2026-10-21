using LiveLearn.Assessment.API.Dto.QueryParameters;
using LiveLearn.Assessment.API.Extensions;
using LiveLearn.Assessment.Application.Commands;
using LiveLearn.Assessment.Application.Queries;
using LiveLearn.BuildingBlocks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveLearn.Assessment.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class TasksController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTasksByTutor(bool includeArchived, [FromQuery] PageableQueryParams query, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return this.MisconfiguredTokenProblem();

        var result = await mediator.Send(new GetTasksByTutorQuery(tutorId, includeArchived, query.PageNumber, query.PageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpPost]
    [Route("{taskId:guid}/archive")]
    public async Task<IActionResult> ArchiveTask(Guid taskId, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return this.MisconfiguredTokenProblem();

        var result = await mediator.Send(new ArchiveTaskCommand(tutorId, taskId), ct);

        return result.IsSuccess ? NoContent() : this.ToProblemResult(result.Errors);
    }
}
