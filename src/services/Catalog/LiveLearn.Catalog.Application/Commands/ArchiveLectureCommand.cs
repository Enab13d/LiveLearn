using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;


public sealed record ArchiveLectureCommand(Guid CourseId, Guid SectionId, Guid LectureId) : ICommand;
