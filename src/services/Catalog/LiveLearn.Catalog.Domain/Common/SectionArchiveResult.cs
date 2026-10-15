namespace LiveLearn.Catalog.Domain.Common;


internal sealed record SectionArchiveResult(IReadOnlyList<Guid> LectureIds, IReadOnlyList<Guid> TaskIds);
