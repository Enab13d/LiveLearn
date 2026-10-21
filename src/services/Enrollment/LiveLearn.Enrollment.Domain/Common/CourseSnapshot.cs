namespace LiveLearn.Enrollment.Domain.Common;

// The lectures and tasks active in a course at Revision, built from the structure projection.
public sealed record CourseSnapshot(long Revision, IReadOnlyList<Guid> LectureIds, IReadOnlyList<Guid> TaskIds);
