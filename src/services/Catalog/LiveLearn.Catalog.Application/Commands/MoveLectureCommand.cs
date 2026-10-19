using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;


public sealed record MoveLectureCommand(Guid CourseId, Guid TutorId, Guid SectionId, Guid LectureId, Guid TargetSectionId) : ICommand;
