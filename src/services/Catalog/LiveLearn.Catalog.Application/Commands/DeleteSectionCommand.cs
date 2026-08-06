using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record DeleteSectionCommand(Guid CourseId, Guid TutorId, Guid SectionId) : ICommand;
