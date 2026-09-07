
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;


public sealed record UnassignTaskCommand(Guid TaskId) : ICommand;
