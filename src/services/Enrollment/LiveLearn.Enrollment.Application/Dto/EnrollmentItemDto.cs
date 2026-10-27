namespace LiveLearn.Enrollment.Application.Dto;

public sealed record EnrollmentItemDto(Guid ItemId, string Kind, DateTimeOffset? CompletedAt);
