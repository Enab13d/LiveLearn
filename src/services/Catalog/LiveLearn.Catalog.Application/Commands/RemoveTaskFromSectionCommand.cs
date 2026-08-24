

using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;


public sealed record RemoveTaskFromSectionCommand(Guid TutorId, Guid CourseId, Guid SectionId, Guid TaskId) : ICommand;
