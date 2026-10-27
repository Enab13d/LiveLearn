using LiveLearn.Identity.API.Extensions;
using LiveLearn.Identity.Application.Authorization;
using LiveLearn.Identity.Application.Commands;
using LiveLearn.Identity.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveLearn.Identity.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class UsersController(ISender mediator) : ControllerBase
{
    [Authorize(Policy = AuthorizationPolicies.AdminPolicy)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserProfile(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserProfileQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfileData(CancellationToken ct)
    {
        var result = await mediator.Send(new GetCurrentUserProfileQuery(), ct);
        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncAuthData(CancellationToken ct)
    {
        var result = await mediator.Send(new ProvisionUserCommand(), ct);
        return result.IsSuccess ? NoContent() : this.ToProblemResult(result.Errors);
    }
}
