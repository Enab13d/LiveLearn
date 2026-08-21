namespace LiveLearn.Contracts.Catalog;

public sealed record SectionRemovedEvent(Guid CourseId, Guid SectionId, IReadOnlyList<Guid> TaskIds);
