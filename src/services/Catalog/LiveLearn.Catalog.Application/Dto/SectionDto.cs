namespace LiveLearn.Catalog.Application.Dto;

public sealed record SectionDto(Guid Id, string Title, int Order, IReadOnlyList<LectureDto> Lectures);
