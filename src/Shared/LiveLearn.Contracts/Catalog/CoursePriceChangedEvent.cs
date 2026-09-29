namespace LiveLearn.Contracts.Catalog;


public sealed record CoursePriceChangedEvent(Guid CourseId, decimal NewPrice, DateTimeOffset OccurredOn);
