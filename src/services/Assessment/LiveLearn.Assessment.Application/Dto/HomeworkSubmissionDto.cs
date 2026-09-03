namespace LiveLearn.Assessment.Application.Dto;


public sealed record HomeworkSubmissionDto(
    Guid Id,
    Guid StudentId,
    Guid SectionId,
    Guid CourseId,
    string Content,
    string Status,
    string? InstructorFeedback,
    DateTimeOffset SubmittedAt
);
