namespace LiveLearn.Assessment.Application.Dto;


public sealed record TaskSummaryDto(Guid Id, string Title, string TaskType, DateTimeOffset? ArchivedAt);
