using LiveLearn.Ordering.API.Dto.QueryParameters;
using LiveLearn.Ordering.API.Extensions;
using LiveLearn.Ordering.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveLearn.Ordering.API.Controllers;


[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMyOrders([FromQuery] PageableQueryParams query, CancellationToken ct)
    {
        var result = await mediator.Send(new GetMyOrdersQuery(query.Page, query.PageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetOrderByIdQuery(id), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }
}
