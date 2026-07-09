namespace LiveLearn.Catalog.Application.Dto;

public sealed record SectionDto(string Title, int Order, IReadOnlyList<LectureDto> Lectures);
