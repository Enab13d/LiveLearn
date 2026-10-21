namespace LiveLearn.Assessment.Application.Dto;


public sealed record QuizAttemptDto(Guid Id, Guid CourseId, int Score, bool IsPassed, DateTimeOffset AttemptedAt);
