using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;


public sealed record MarkLectureVideoFailedCommand(
    Guid CourseId,
    Guid LectureId,
    Guid VideoId) : ICommand;
