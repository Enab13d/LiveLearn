using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record MarkLectureVideoUploadedCommand(Guid CourseId, Guid TutorId, Guid LectureId, Guid VideoId) : ICommand;
