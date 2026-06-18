using System.Security.Claims;
using LiveLearn.Identity.API.Extensions;
using LiveLearn.Identity.Application.Commands;
using LiveLearn.Identity.Application.Queries;
using LiveLearn.Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveLearn.Identity.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class UsersController(ISender mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserProfile(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserProfileQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncAuthData(CancellationToken ct)
    {
        var claims = HttpContext.User;
        var role = User.FindAll("role")
            .Select(c => Enum.TryParse<Role>(c.Value, out var r) ? (Role?)r : null)
            .FirstOrDefault(r => r.HasValue);

        if (role is null)
            return Problem("Token contains no valid application role. Check Keycloak role mapper configuration.");

        string sub = claims.FindFirstValue("sub") ?? "";
        string firstName = claims.FindFirstValue("given_name") ?? "";
        string lastName = claims.FindFirstValue("family_name") ?? "";
        string email = claims.FindFirstValue("email") ?? "";
        if (!Guid.TryParse(sub, out var id))
        {
            return Problem($"Sub claim {sub} is not a valid Guid");
        }
        var result = await mediator.Send(new ProvisionUserCommand(id, email, firstName, lastName, role.Value), ct);
        return result.IsSuccess ? NoContent() : this.ToProblemResult(result.Errors);
    }
}
