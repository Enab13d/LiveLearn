namespace LiveLearn.Contracts.Catalog;


// ObjectKey is relative to the raw-videos bucket.
public sealed record LectureVideoUploadedEvent(
    Guid CourseId,
    Guid LectureId,
    Guid VideoId,
    string ObjectKey,
    DateTimeOffset OccurredOn);
