using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.DomainEvents;


// Carries the lectures and task links archived together with the section in the same revision.
public sealed record SectionArchivedDomainEvent(
    Guid CourseId,
    Guid SectionId,
    long Revision,
    IReadOnlyList<Guid> ArchivedLectureIds,
    IReadOnlyList<Guid> ArchivedTaskIds) : IDomainEvent;
