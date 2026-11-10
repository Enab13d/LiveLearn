using LiveLearn.Ordering.API.Dto.Requests;
using LiveLearn.Ordering.API.Extensions;
using LiveLearn.Ordering.Application.Commands;
using LiveLearn.Ordering.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveLearn.Ordering.API.Controllers;


// The current student's cart (O2, O11).
[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class CartController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken ct)
    {
        var result = await mediator.Send(new GetCartQuery(), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] RequestAddCartItemDto request, CancellationToken ct)
    {
        var result = await mediator.Send(new AddToCartCommand(request.CourseId), ct);

        return result.IsSuccess ? NoContent() : this.ToProblemResult(result.Errors);
    }

    [HttpDelete("items/{courseId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid courseId, CancellationToken ct)
    {
        var result = await mediator.Send(new RemoveFromCartCommand(courseId), ct);

        return result.IsSuccess ? NoContent() : this.ToProblemResult(result.Errors);
    }

    [HttpDelete]
    public async Task<IActionResult> Clear(CancellationToken ct)
    {
        var result = await mediator.Send(new ClearCartCommand(), ct);

        return result.IsSuccess ? NoContent() : this.ToProblemResult(result.Errors);
    }
}
