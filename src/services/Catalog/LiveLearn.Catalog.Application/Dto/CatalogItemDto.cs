namespace LiveLearn.Catalog.Application.Dto;

public sealed record CatalogItemDto(
    Guid Id,
    Guid TutorId,
    Guid CategoryId,
    string Title,
    string ThumbnailUrl,
    decimal Price
);
