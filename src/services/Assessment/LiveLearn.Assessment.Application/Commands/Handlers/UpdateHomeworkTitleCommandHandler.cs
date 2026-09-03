using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands.Handlers;


internal sealed class UpdateHomeworkTitleCommandHandler(
    IAssessmentTaskRepository repository,
    IUnitOfWork unitOfWork
) : ICommandHandler<UpdateHomeworkTitleCommand>
{
    public async Task<Result> Handle(UpdateHomeworkTitleCommand request, CancellationToken ct)
    {
        var (homeworkId, tutorId, title) = request;
        var homework = await repository.GetHomeworkAsync(homeworkId, ct);
        if (homework is null) return Result.Failure(TaskErrors.NotFound);
        if (homework.TutorId != tutorId) return Result.Failure(TaskErrors.Forbidden);
        homework.UpdateTitle(title);
        await unitOfWork.CommitAsync(ct);
        return Result.Success();
    }
}
