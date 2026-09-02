using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;


public sealed record CreateQuizCommand(Guid TutorId, string Title, int PassingScore) : ICommand<Guid>;
