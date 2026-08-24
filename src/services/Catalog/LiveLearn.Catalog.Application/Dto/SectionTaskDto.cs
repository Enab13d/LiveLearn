namespace LiveLearn.Catalog.Application.Dto;

public sealed record SectionTaskDto(Guid SectionId, Guid CourseId, Guid TaskId, string TaskType);
