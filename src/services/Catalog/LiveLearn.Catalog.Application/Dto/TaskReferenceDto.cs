namespace LiveLearn.Catalog.Application.Dto;

public sealed record TaskReferenceDto(Guid TaskId, Guid TutorId, string TaskType);
