using LiveLearn.Assessment.API.Dto.QueryParameters;
using LiveLearn.Assessment.API.Extensions;
using LiveLearn.Assessment.Application.Commands;
using LiveLearn.Assessment.Application.Queries;
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
        var result = await mediator.Send(new GetTasksByTutorQuery(includeArchived, query.PageNumber, query.PageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpPost]
    [Route("{taskId:guid}/archive")]
    public async Task<IActionResult> ArchiveTask(Guid taskId, CancellationToken ct)
    {
        var result = await mediator.Send(new ArchiveTaskCommand(taskId), ct);

        return result.IsSuccess ? NoContent() : this.ToProblemResult(result.Errors);
    }
}
