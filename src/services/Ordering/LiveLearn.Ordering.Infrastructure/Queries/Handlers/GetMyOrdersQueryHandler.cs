using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Application.Dto;
using LiveLearn.Ordering.Application.Queries;
using LiveLearn.Ordering.Application.Services;
using LiveLearn.Ordering.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Ordering.Infrastructure.Queries.Handlers;


internal sealed class GetMyOrdersQueryHandler(ICurrentUser currentUser, ReadDbContext dbContext) : IQueryHandler<GetMyOrdersQuery, PagedResult<OrderDto>>
{
    public async Task<Result<PagedResult<OrderDto>>> Handle(GetMyOrdersQuery request, CancellationToken ct)
    {
        var query = dbContext.Orders.Where(e => e.StudentId == currentUser.UserId);

        int totalCount = await query.CountAsync(ct);

        var orders = await query
            .OrderByDescending(e => e.PlacedAt)
            .ThenBy(e => e.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new OrderDto(
                e.Id,
                e.PlacedAt,
                e.Status,
                e.Total,
                e.Lines.Select(l => new OrderLineDto(l.Id, l.CourseId, l.TutorId, l.Price, l.Status)).ToList()))
            .ToListAsync(ct);

        return new PagedResult<OrderDto>(orders, totalCount, request.Page, request.PageSize);
    }
}
