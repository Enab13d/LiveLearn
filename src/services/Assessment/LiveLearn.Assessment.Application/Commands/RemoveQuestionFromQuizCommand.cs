

using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;


public sealed record RemoveQuestionFromQuizCommand(Guid QuizId, Guid TutorId, Guid QuestionId) : ICommand;
