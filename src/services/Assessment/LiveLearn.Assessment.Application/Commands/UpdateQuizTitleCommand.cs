
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;

public sealed record UpdateQuizTitleCommand(Guid TutorId, Guid QuizId, string NewTitle) : ICommand;
