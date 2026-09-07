
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;


public sealed record AssignTaskToCourseAndSectionCommand(Guid TaskId, Guid CourseId, Guid SectionId) : ICommand;
