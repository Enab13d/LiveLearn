using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;

public sealed record ArchiveTaskCommand(Guid TutorId, Guid TaskId) : ICommand;
