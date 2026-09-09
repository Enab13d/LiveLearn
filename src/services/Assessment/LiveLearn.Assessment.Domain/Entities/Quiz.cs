using LiveLearn.Assessment.Domain.Common;
using LiveLearn.Assessment.Domain.DomainEvents;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Domain.Entities;


public sealed class Quiz : AssessmentTask
{

    private Quiz() { }
    public int PassingScore { get; private set; }

    private readonly List<Question> _questions = [];

    public IReadOnlyCollection<Question> Questions => _questions;

    public static Quiz Create(Guid id, string title, Guid tutorId, int passingPercent)
    {

        Quiz quiz = new()
        {
            Id = id,
            Title = title,
            TutorId = tutorId,
            PassingScore = passingPercent
        };

        quiz.RaiseDomainEvent(new TaskCreatedDomainEvent(id, tutorId, nameof(Quiz)));
        return quiz;
    }

    public Result AddQuestion(string text, List<string> answerContents, int correctAnswerIdx)
    {
        if (answerContents.Count - 1 < correctAnswerIdx || correctAnswerIdx < 0)
            return Result.Failure(QuizErrors.InvalidAnswerIndex);

        Question question = new(text, answerContents, correctAnswerIdx);
        _questions.Add(question);

        return Result.Success();
    }

    public Result RemoveQuestion(Guid questionId)
    {
        var question = _questions.FirstOrDefault(e => e.Id == questionId);

        if (question is null) return Result.Failure(QuizErrors.QuestionNotFound);

        _questions.Remove(question);

        return Result.Success();
    }

    public Result<QuizEvaluationResult> Evaluate(Dictionary<Guid, Guid> questionAnswerMap, Guid studentId)
    {
        if (SectionId is null || CourseId is null)
            return Result<QuizEvaluationResult>.Failure(TaskErrors.UnassignedTaskEvaluation);

        int totalQuestions = _questions.Count;
        if (totalQuestions <= 0) return Result<QuizEvaluationResult>.Failure(QuizErrors.EmptyQuizEvaluation);

        List<Guid> correctAnswerIds = [];
        List<Guid> wrongAnswerIds = [];

        foreach (var record in questionAnswerMap)
        {
            var question = _questions.FirstOrDefault(e => e.Id == record.Key);
            if (question is null) return Result<QuizEvaluationResult>.Failure(QuizErrors.QuestionNotFound);


            if (question.CorrectAnswerId == record.Value)
            {
                correctAnswerIds.Add(record.Value);
            }
            else
            {
                wrongAnswerIds.Add(record.Value);
            }

        }

        var score = (int)Math.Round(100.0 * correctAnswerIds.Count / totalQuestions);
        bool passed = score >= PassingScore;

        if (passed)
        {
            RaiseDomainEvent(new TaskCompletedDomainEvent(Id, studentId, SectionId.Value, CourseId.Value));
        }
        else
        {
            RaiseDomainEvent(new TaskFailedDomainEvent(Id, studentId, SectionId.Value, CourseId.Value));
        }

        return new QuizEvaluationResult(passed, score, correctAnswerIds, wrongAnswerIds);

    }

    public void UpdateTitle(string newTitle)
    {
        Title = newTitle;
    }

}
