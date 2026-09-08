namespace LiveLearn.Assessment.Application.Dto;


public sealed record TaskSummaryDto(Guid Id, string Title, string TaskType, Guid? SectionId, Guid? CourseId);
