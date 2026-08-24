namespace LiveLearn.Contracts.Catalog;


public sealed record TaskAssignedToSectionEvent(
    Guid TaskId,
    Guid SectionId,
    Guid CourseId,
    DateTimeOffset OccurredOn
);
