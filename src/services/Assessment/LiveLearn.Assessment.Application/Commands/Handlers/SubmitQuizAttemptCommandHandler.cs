using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Application.Services;
using LiveLearn.Assessment.Domain.Common;
using LiveLearn.Assessment.Domain.Entities;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands.Handlers;


internal sealed class SubmitQuizAttemptCommandHandler(
    ICurrentUser currentUser,
    IAssessmentTaskRepository assessmentTaskRepository,
    IQuizAttemptRepository quizAttemptRepository,
    IEnrollmentLookup enrollmentLookup,
    ICourseTaskLookup courseTaskLookup,
    IUnitOfWork unitOfWork
) : ICommandHandler<SubmitQuizAttemptCommand, QuizEvaluationResult>
{
    public async Task<Result<QuizEvaluationResult>> Handle(SubmitQuizAttemptCommand request, CancellationToken ct)
    {
        var studentId = currentUser.UserId;

        var quiz = await assessmentTaskRepository.GetQuizAsync(request.QuizId, ct);
        if (quiz is null) return Result<QuizEvaluationResult>.Failure(TaskErrors.NotFound);

        // archived tasks stay submittable
        if (!await enrollmentLookup.IsEnrolledAsync(studentId, request.CourseId, ct)
            || !await courseTaskLookup.WasEverAssignedAsync(request.CourseId, quiz.Id, ct))
            return Result<QuizEvaluationResult>.Failure(TaskErrors.Forbidden);

        var result = quiz.Evaluate(request.Answers, studentId, request.CourseId);
        if (!result.IsSuccess) return result;

        var quizAttempt = QuizAttempt.Create(
            request.QuizId, studentId, request.CourseId, result.Value.Score, result.Value.IsPassed);

        await quizAttemptRepository.AddAsync(quizAttempt, ct);
        await unitOfWork.CommitAsync(ct);

        return result;

    }
}
