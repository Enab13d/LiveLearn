using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record UpdateSectionOrderCommand(Guid CourseId, Guid TutorId, Guid SectionId, int Order) : ICommand;
