using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Application.Services;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands.Handlers;


internal sealed class AddQuestionToQuizCommandHandler(
    IAssessmentTaskRepository repository,
    ILockedTaskLookup lockedTaskLookup,
    IUnitOfWork unitOfWork
) : ICommandHandler<AddQuestionToQuizCommand>
{
    public async Task<Result> Handle(AddQuestionToQuizCommand request, CancellationToken ct)
    {
        var quiz = await repository.GetQuizAsync(request.QuizId, ct);
        if (quiz is null) return Result.Failure(TaskErrors.NotFound);
        if (quiz.TutorId != request.TutorId) return Result.Failure(TaskErrors.Forbidden);
        bool isLocked = await lockedTaskLookup.IsLockedAsync(quiz.Id, ct);
        if (isLocked) return Result.Failure(TaskErrors.Locked);
        var result = quiz.AddQuestion(request.Text, request.Answers, request.CorrectAnswerIdx);
        if (result.IsSuccess) await unitOfWork.CommitAsync(ct);
        return result;

    }
}
