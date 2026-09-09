using LiveLearn.Assessment.API.Dto.QueryParameters;
using LiveLearn.Assessment.API.Extensions;
using LiveLearn.Assessment.Application.Authorization;
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
    [Authorize(Policy = AuthorizationPolicies.TutorPolicy)]
    [HttpGet]
    public async Task<IActionResult> GetTasksByTutor(bool assigned, [FromQuery] PageableQueryParams query, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return this.MisconfiguredTokenProblem();

        var result = await mediator.Send(new GetTasksByTutorQuery(tutorId, assigned, query.PageNumber, query.PageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpDelete]
    [Route("{taskId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.TutorPolicy)]
    public async Task<IActionResult> DeleteTask(Guid taskId, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return this.MisconfiguredTokenProblem();

        var result = await mediator.Send(new DeleteTaskCommand(tutorId, taskId), ct);

        return result.IsSuccess ? NoContent() : this.ToProblemResult(result.Errors);
    }
}
