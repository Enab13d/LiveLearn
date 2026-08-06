using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record UpdateSectionCommand(Guid CourseId, Guid TutorId, Guid SectionId, string Title) : ICommand;
