using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;

public sealed record DeleteTaskCommand(Guid TutorId, Guid TaskId) : ICommand;
