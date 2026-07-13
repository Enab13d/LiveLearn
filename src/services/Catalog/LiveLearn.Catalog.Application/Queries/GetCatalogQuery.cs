using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Queries;

public sealed record GetCatalogQuery(
    int PageNumber,
    int PageSize,
    Guid? CategoryId = null,
    Guid? TutorId = null,
    decimal? MaxPrice = null
) : IQuery<PagedResult<CatalogItemDto>>;
