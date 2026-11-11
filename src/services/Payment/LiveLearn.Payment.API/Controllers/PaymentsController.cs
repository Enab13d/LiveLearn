using LiveLearn.Payment.API.Extensions;
using LiveLearn.Payment.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveLearn.Payment.API.Controllers;


[Authorize]
[ApiController]
[Route("payments")]
public sealed class PaymentsController(ISender mediator) : ControllerBase
{
    [HttpGet("orders/{orderId:guid}/checkout")]
    public async Task<IActionResult> GetCheckout(Guid orderId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCheckoutQuery(orderId), ct);

        if (!result.IsSuccess) return this.ToProblemResult(result.Errors);

        // No URL yet: the session is still being created, the client polls again (D16).
        return result.Value.Url is null ? Accepted() : Ok(result.Value);
    }
}
