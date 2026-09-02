namespace LiveLearn.Assessment.Application.Dto;


public sealed record HomeworkDto(Guid Id, Guid TutorId, string Title, string Description);
