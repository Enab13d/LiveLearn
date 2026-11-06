namespace LiveLearn.Contracts.Enrollment;

public sealed record EnrollmentFailedEvent(
    Guid OrderId,
    Guid OrderLineId,
    Guid StudentId,
    Guid CourseId,
    string Reason,
    DateTimeOffset OccurredOn
);
