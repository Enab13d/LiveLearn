namespace LiveLearn.Assessment.Application.Dto;


public sealed record QuizAttemptDto(Guid Id, Guid SectionId, Guid CourseId, int Score, bool IsPassed, DateTimeOffset AttemptedAt);
