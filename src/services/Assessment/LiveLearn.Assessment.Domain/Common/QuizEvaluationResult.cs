namespace LiveLearn.Assessment.Domain.Common;


public sealed record QuizEvaluationResult(
    bool IsPassed,
    int Score,
    List<Guid> CorrectAnswerIds,
    List<Guid> WrongAnswerIds
);
