using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands.Handlers;


internal sealed class UpdateQuizTitleCommandHandler(
    IAssessmentTaskRepository repository,
    IUnitOfWork unitOfWork
) : ICommandHandler<UpdateQuizTitleCommand>
{
    public async Task<Result> Handle(UpdateQuizTitleCommand request, CancellationToken ct)
    {
        var quiz = await repository.GetQuizAsync(request.QuizId, ct);
        if (quiz is null) return Result.Failure(TaskErrors.NotFound);
        if (quiz.TutorId != request.TutorId) return Result.Failure(TaskErrors.Forbidden);
        quiz.UpdateTitle(request.NewTitle);
        await unitOfWork.CommitAsync(ct);
        return Result.Success();
    }
}
