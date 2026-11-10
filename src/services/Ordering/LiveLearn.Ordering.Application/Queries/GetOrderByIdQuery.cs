using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Application.Dto;

namespace LiveLearn.Ordering.Application.Queries;


// Own order only: another student's order returns not found (O11).
public sealed record GetOrderByIdQuery(Guid OrderId) : IQuery<OrderDto>;
