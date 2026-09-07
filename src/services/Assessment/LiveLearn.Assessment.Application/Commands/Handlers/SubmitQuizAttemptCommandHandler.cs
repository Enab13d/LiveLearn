using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Domain.Common;
using LiveLearn.Assessment.Domain.Entities;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands.Handlers;


internal sealed class SubmitQuizAttemptCommandHandler(
    IAssessmentTaskRepository assessmentTaskRepository,
    IQuizAttemptRepository quizAttemptRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<SubmitQuizAttemptCommand, QuizEvaluationResult>
{
    public async Task<Result<QuizEvaluationResult>> Handle(SubmitQuizAttemptCommand request, CancellationToken ct)
    {

        var quiz = await assessmentTaskRepository.GetQuizAsync(request.QuizId, ct);
        if (quiz is null) return Result<QuizEvaluationResult>.Failure(TaskErrors.NotFound);

        var result = quiz.Evaluate(request.Answers, request.StudentId);
        if (!result.IsSuccess) return result;

        // null-forgiving operator used below, since quiz.Evaluate() already has guard against null 
        var quizAttempt = QuizAttempt.Create(
            request.QuizId, request.StudentId, quiz.SectionId!.Value, quiz.CourseId!.Value, result.Value.Score, result.Value.IsPassed);

        await quizAttemptRepository.AddAsync(quizAttempt, ct);
        await unitOfWork.CommitAsync(ct);

        return result;

    }
}
