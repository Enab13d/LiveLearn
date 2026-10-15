namespace LiveLearn.Contracts.Assessment;

public sealed record TaskArchivedEvent(Guid TaskId, DateTimeOffset OccurredOn);
