using LiveLearn.Assessment.Domain.Common;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;


public sealed record SubmitQuizAttemptCommand(Guid StudentId, Guid QuizId, Dictionary<Guid, Guid> Answers) : ICommand<QuizEvaluationResult>;
