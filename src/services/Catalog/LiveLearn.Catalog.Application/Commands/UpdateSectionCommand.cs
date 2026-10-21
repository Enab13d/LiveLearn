using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record UpdateSectionCommand(Guid CourseId, Guid SectionId, string Title) : ICommand;
