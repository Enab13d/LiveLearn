namespace LiveLearn.Contracts.Streaming;


public sealed record VideoTranscodingCompletedEvent(
    Guid VideoId,
    Guid LectureId,
    Guid CourseId,
    TimeSpan Duration,
    DateTimeOffset OccurredOn
);
