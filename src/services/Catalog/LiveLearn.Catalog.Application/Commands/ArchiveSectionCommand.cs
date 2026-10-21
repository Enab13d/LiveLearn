using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;


public sealed record ArchiveSectionCommand(Guid CourseId, Guid SectionId) : ICommand;
