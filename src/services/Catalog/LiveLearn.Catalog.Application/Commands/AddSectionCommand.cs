using LiveLearn.BuildingBlocks;


namespace LiveLearn.Catalog.Application.Commands;

public sealed record AddSectionCommand(Guid SectionId, Guid CourseId, Guid TutorId, string Title, int Order) : ICommand<Guid>;



