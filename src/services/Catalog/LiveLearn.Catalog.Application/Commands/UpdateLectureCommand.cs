using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record UpdateLectureCommand(Guid CourseId, Guid SectionId, Guid LectureId, string Title, string Description) : ICommand;
