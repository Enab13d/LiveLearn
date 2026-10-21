namespace LiveLearn.Assessment.Application.Dto;


public sealed record HomeworkSubmissionDto(
    Guid Id,
    Guid StudentId,
    Guid CourseId,
    string Content,
    string Status,
    string? InstructorFeedback,
    DateTimeOffset SubmittedAt
);
