namespace LiveLearn.Contracts.Assessment;


public sealed record TaskFailedEvent(Guid StudentId, Guid TaskId, Guid SectionId, Guid CourseId, DateTimeOffset OccurredOn);
