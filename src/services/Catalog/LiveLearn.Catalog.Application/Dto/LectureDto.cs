namespace LiveLearn.Catalog.Application.Dto;

public sealed record LectureDto(Guid Id, string Title, string Type, int Order, int? DurationInSeconds);


