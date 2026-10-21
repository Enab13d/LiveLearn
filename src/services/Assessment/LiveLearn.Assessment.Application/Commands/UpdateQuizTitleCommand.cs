
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;

public sealed record UpdateQuizTitleCommand(Guid QuizId, string NewTitle) : ICommand;
