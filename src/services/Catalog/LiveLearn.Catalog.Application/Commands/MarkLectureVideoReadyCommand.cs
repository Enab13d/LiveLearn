using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;


public sealed record MarkLectureVideoReadyCommand(
    Guid CourseId,
    Guid LectureId,
    Guid VideoId,
    TimeSpan Duration) : ICommand;
