namespace LiveLearn.Contracts.Assessment;


public sealed record TaskFailedEvent(Guid StudentId, Guid TaskId, Guid CourseId, DateTimeOffset OccurredOn);
