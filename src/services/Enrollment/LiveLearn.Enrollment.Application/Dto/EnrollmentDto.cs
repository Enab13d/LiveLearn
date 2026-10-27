namespace LiveLearn.Enrollment.Application.Dto;

public sealed record EnrollmentDto(
    Guid Id,
    Guid CourseId,
    string Status,
    long CourseRevision,
    DateTimeOffset EnrolledAt,
    DateTimeOffset? CompletedAt,
    int CompletedCount,
    int TotalCount
);
