namespace LiveLearn.Catalog.Application.Dto;

public sealed record CourseDto(
    Guid Id,
    Guid TutorId,
    Guid CategoryId,
    string Title,
    string Description,
    string ThumbnailUrl,
    decimal Price,
    string Status
);
