namespace LiveLearn.Contracts.Catalog;


// One message per Course.Revision; a revision with no lecture/task change (AddSection) carries empty lists
// so consumers see a gap-free revision sequence.
public sealed record CourseStructureRevisedEvent(
    Guid CourseId,
    long Revision,
    IReadOnlyList<Guid> AddedLectureIds,
    IReadOnlyList<Guid> ArchivedLectureIds,
    IReadOnlyList<Guid> AddedTaskIds,
    IReadOnlyList<Guid> ArchivedTaskIds,
    DateTimeOffset OccurredOn);
