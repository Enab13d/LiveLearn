namespace LiveLearn.Contracts.Assessment;


public sealed record TaskCreatedEvent(Guid TaskId, Guid TutorId, string TaskType, DateTimeOffset OccurredOn);
