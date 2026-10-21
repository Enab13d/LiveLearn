using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;


public sealed record CreateQuizCommand(string Title, int PassingScore) : ICommand<Guid>;
