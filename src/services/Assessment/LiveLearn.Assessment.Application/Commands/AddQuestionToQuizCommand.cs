

using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;

public sealed record AddQuestionToQuizCommand(
    Guid TutorId,
    Guid QuizId,
    string Text,
    List<string> Answers,
    int CorrectAnswerIdx
) : ICommand;
