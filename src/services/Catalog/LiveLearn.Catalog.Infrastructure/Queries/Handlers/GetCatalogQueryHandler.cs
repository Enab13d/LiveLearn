using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Queries;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Queries.Handlers;


internal sealed class GetCatalogQueryHandler(ReadDbContext dbContext) : IQueryHandler<GetCatalogQuery, PagedResult<CatalogItemDto>>
{
    public async Task<Result<PagedResult<CatalogItemDto>>> Handle(GetCatalogQuery request, CancellationToken ct)
    {
        var query = dbContext.Courses
            .Select(e => new CatalogItemDto(e.Id, e.TutorId, e.CategoryId, e.Title, e.ThumbnailUrl, e.Price));

        if (request.CategoryId is not null)
            query = query.Where(e => e.CategoryId == request.CategoryId);

        if (request.TutorId is not null)
            query = query.Where(e => e.TutorId == request.TutorId);

        if (request.MaxPrice is not null)
            query = query.Where(e => e.Price <= request.MaxPrice);

        int totalCount = await query.CountAsync(ct);

        if (request.PageNumber > 0 && request.PageSize > 0)
            query = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var courses = await query
            .ToListAsync(ct);

        return new PagedResult<CatalogItemDto>(courses, totalCount, request.PageNumber, request.PageSize);
    }
}
