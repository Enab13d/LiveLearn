using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record MarkLectureVideoUploadedCommand(Guid CourseId, Guid LectureId, Guid VideoId) : ICommand;
