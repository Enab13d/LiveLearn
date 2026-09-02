using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;


public sealed record CreateHomeworkCommand(Guid TutorId, string Title, string Description) : ICommand<Guid>;
