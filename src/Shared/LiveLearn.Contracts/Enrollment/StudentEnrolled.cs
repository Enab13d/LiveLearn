namespace LiveLearn.Contracts.Enrollment;

// The purchase saga's success reply to EnrollStudent; projections use EnrollmentActivatedEvent instead.
public sealed record StudentEnrolled(
    Guid OrderId,
    Guid OrderLineId,
    Guid StudentId,
    Guid CourseId,
    DateTimeOffset OccurredOn
);
