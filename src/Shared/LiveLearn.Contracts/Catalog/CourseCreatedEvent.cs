namespace LiveLearn.Contracts.Catalog;


public sealed record CourseCreatedEvent(Guid CourseId, Guid OwnerId, DateTimeOffset OccurredOn);
