using LiveLearn.Ordering.API.Dto.QueryParameters;
using LiveLearn.Ordering.API.Dto.Requests;
using LiveLearn.Ordering.API.Extensions;
using LiveLearn.Ordering.Application.Commands;
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

    // 201 { orderId }; 409 when the total changed, 422 when no cart item is available (O5, O11).
    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] RequestPlaceOrderDto request, CancellationToken ct)
    {
        var result = await mediator.Send(new PlaceOrderCommand(request.ExpectedTotal), ct);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetOrderById), new { id = result.Value }, new { orderId = result.Value })
            : this.ToProblemResult(result.Errors);
    }
}
