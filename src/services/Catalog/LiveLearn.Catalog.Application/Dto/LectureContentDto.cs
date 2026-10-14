namespace LiveLearn.Catalog.Application.Dto;


public sealed record LectureContentDto(Guid Id, string Title, string Description, string Type, int Order, int? DurationInSeconds, Guid? VideoId);
