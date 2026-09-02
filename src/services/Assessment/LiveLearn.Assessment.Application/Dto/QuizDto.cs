namespace LiveLearn.Assessment.Application.Dto;

public sealed record QuizDto(Guid Id, string Title, IReadOnlyList<QuestionDto> Questions);
