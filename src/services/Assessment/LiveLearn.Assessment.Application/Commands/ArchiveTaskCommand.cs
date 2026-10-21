using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;

public sealed record ArchiveTaskCommand(Guid TaskId) : ICommand;
