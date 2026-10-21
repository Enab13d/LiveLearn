namespace LiveLearn.Contracts.Assessment;


// A pass counts only for CourseId — the course the submission was made in.
public sealed record TaskCompletedEvent(
    Guid StudentId,
    Guid TaskId,
    Guid CourseId,
    DateTimeOffset OccurredOn
);
