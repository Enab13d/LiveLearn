using LiveLearn.Identity.API.Extensions;
using LiveLearn.Identity.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveLearn.Identity.API.Controllers;

[Authorize]
[ApiController]
[Route("api/identity/[controller]")]
public sealed class UsersController(ISender mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserProfile(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserProfileQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }
}
