using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;


public sealed record ArchiveLectureCommand(Guid CourseId, Guid TutorId, Guid SectionId, Guid LectureId) : ICommand;
