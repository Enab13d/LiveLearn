using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Queries;

public sealed record GetCategoriesQuery : IQuery<IReadOnlyList<CategoryDto>>;
