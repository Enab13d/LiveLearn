using LiveLearn.BuildingBlocks;


namespace LiveLearn.Catalog.Application.Commands;

public sealed record AddSectionCommand(Guid CourseId, Guid TutorId, string Title) : ICommand<Guid>;



