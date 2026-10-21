

using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;


public sealed record AssignTaskToSectionCommand( Guid CourseId, Guid SectionId, Guid TaskId) : ICommand;
