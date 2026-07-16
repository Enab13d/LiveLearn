using LiveLearn.Catalog.API.Extensions;
using LiveLearn.Catalog.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveLearn.Catalog.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class CategoriesController(ISender mediator) : ControllerBase
{

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
    {
        var result = await mediator.Send(new GetCategoriesQuery(), ct);
        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }
}
