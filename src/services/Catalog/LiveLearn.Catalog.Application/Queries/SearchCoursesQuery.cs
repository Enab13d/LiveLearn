using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Queries;

public sealed record SearchCoursesQuery(string Query, int PageNumber, int PageSize) : IQuery<PagedResult<CatalogItemDto>>;
