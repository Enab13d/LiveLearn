using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Queries;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace LiveLearn.Catalog.Infrastructure.Queries.Handlers;


internal sealed class GetCatalogQueryHandler(ReadDbContext dbContext) : IQueryHandler<GetCatalogQuery, PagedResult<CatalogItemDto>>
{
    public async Task<Result<PagedResult<CatalogItemDto>>> Handle(GetCatalogQuery request, CancellationToken ct)
    {
        var query = dbContext.Courses.AsQueryable();

        if (request.CategoryId is not null)
            query = query.Where(e => e.CategoryId == request.CategoryId);

        if (request.TutorId is not null)
            query = query.Where(e => e.TutorId == request.TutorId);

        if (request.MaxPrice is not null)
            query = query.Where(e => e.Price <= request.MaxPrice);

        if (request.Query is not null)
        {
            query = query
                    .Where(e => EF.Property<NpgsqlTsVector>(e, "SearchVector")
                        .Matches(EF.Functions.WebSearchToTsQuery("english", request.Query)))
                    .OrderByDescending(e => EF.Property<NpgsqlTsVector>(e, "SearchVector")
                        .Rank(EF.Functions.WebSearchToTsQuery("english", request.Query)));
        }



        int totalCount = await query.CountAsync(ct);

        var courses = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new CatalogItemDto(e.Id, e.TutorId, e.CategoryId, e.Title, e.ThumbnailUrl, e.Price))
            .ToListAsync(ct);

        return new PagedResult<CatalogItemDto>(courses, totalCount, request.PageNumber, request.PageSize);
    }
}
