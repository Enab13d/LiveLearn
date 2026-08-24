namespace LiveLearn.Contracts.Catalog;


public sealed record TaskRemovedFromSectionEvent(
    Guid TaskId,
    Guid SectionId,
    Guid CourseId,
    DateTimeOffset OccurredOn
);
