using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Queries;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace LiveLearn.Catalog.Infrastructure.Queries.Handlers;


internal sealed class SearchCoursesQueryHandler(ReadDbContext dbContext) : IQueryHandler<SearchCoursesQuery, PagedResult<CatalogItemDto>>
{
    public async Task<Result<PagedResult<CatalogItemDto>>> Handle(SearchCoursesQuery request, CancellationToken ct)
    {
        IQueryable<CatalogItemDto> query = dbContext.Courses
            .Where(e => EF.Property<NpgsqlTsVector>(e, "SearchVector")
                .Matches(EF.Functions.WebSearchToTsQuery("english", request.Query)))
            .OrderByDescending(e => EF.Property<NpgsqlTsVector>(e, "SearchVector")
                .Rank(EF.Functions.WebSearchToTsQuery("english", request.Query)))
            .Select(e => new CatalogItemDto(e.Id, e.TutorId, e.CategoryId, e.Title, e.ThumbnailUrl, e.Price));

        var totalCount = await query.CountAsync(ct);

        if (request.PageNumber > 0 && request.PageSize > 0)
            query = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize);

        var catalogItems = await query.ToListAsync(ct);

        return new PagedResult<CatalogItemDto>(catalogItems, totalCount, request.PageNumber, request.PageSize);
    }
}
