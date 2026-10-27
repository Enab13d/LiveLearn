namespace LiveLearn.Enrollment.Application.Dto;

// Titles and order are not here: they come from Catalog's learning outline, which the frontend merges by ItemId.
public sealed record EnrollmentDetailDto(
    Guid Id,
    Guid CourseId,
    string Status,
    long CourseRevision,
    DateTimeOffset EnrolledAt,
    DateTimeOffset? CompletedAt,
    int CompletedCount,
    int TotalCount,
    IReadOnlyList<EnrollmentItemDto> Items
);
