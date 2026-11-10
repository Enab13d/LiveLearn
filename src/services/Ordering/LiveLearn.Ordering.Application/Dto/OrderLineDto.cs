namespace LiveLearn.Ordering.Application.Dto;


public sealed record OrderLineDto(
    Guid Id,
    Guid CourseId,
    Guid TutorId,
    decimal Price,
    string Status
);
