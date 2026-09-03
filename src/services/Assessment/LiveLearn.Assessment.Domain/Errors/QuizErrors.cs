using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Domain.Errors;


public static class QuizErrors
{
    public static Error InvalidAnswerIndex => Error.Conflict("QuizErrors.InvalidAnswerIndex", "Index should be within answers range");

    public static Error EmptyQuizEvaluation => Error.Conflict("QuizErrors.EmptyQuizEvaluation", "Cannot evaluate quiz with no questions");

    public static Error QuestionNotFound => Error.NotFound("QuizErrors.QuestionNotFound", "Question not found");

}
