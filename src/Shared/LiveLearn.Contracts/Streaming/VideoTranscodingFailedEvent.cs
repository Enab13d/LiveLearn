namespace LiveLearn.Contracts.Streaming;


public sealed record VideoTranscodingFailedEvent(
    Guid VideoId,
    Guid LectureId,
    Guid CourseId,
    DateTimeOffset OccurredOn
);
