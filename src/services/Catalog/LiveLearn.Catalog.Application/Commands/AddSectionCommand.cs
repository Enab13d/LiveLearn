using LiveLearn.BuildingBlocks;


namespace LiveLearn.Catalog.Application.Commands;

public sealed record AddSectionCommand(Guid CourseId, string Title) : ICommand<Guid>;



