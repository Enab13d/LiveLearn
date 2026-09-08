using LiveLearn.Assessment.API.Extensions;
using LiveLearn.Assessment.Application.Authorization;
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
    public async Task<IActionResult> GetTasksByTutor(bool assigned, int pageNumber, int pageSize, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(new GetTasksByTutorQuery(tutorId, assigned, pageNumber, pageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }
}
