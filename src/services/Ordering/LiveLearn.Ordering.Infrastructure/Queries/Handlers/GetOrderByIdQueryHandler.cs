using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Application.Dto;
using LiveLearn.Ordering.Application.Queries;
using LiveLearn.Ordering.Application.Services;
using LiveLearn.Ordering.Domain.Errors;
using LiveLearn.Ordering.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Ordering.Infrastructure.Queries.Handlers;


internal sealed class GetOrderByIdQueryHandler(ICurrentUser currentUser, ReadDbContext dbContext) : IQueryHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var order = await dbContext.Orders
            .Where(e => e.Id == request.OrderId && e.StudentId == currentUser.UserId)
            .Select(e => new OrderDto(
                e.Id,
                e.PlacedAt,
                e.Status,
                e.Total,
                e.Lines.Select(l => new OrderLineDto(l.Id, l.CourseId, l.TutorId, l.Price, l.Status)).ToList()))
            .FirstOrDefaultAsync(ct);

        if (order is null) return Result<OrderDto>.Failure(OrderErrors.NotFound);
        return order;
    }
}
