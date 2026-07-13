namespace LiveLearn.Catalog.Application.Dto;

public sealed record CourseDetailDto(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    Guid TutorId,
    string Title,
    string Description,
    string ThumbnailUrl,
    decimal Price,
    string Status,
    IReadOnlyList<SectionDto> Sections
);
