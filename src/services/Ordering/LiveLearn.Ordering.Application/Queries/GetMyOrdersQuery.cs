using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Application.Dto;

namespace LiveLearn.Ordering.Application.Queries;


// The caller's orders, newest first, all statuses (O11).
public sealed record GetMyOrdersQuery(int Page, int PageSize) : IQuery<PagedResult<OrderDto>>;
