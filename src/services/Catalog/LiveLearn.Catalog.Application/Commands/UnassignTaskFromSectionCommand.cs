using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;


public sealed record UnassignTaskFromSectionCommand( Guid CourseId, Guid SectionId, Guid TaskId) : ICommand;
