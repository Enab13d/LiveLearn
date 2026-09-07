using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Application.Services;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands.Handlers;


internal sealed class UpdateHomeworkDescriptionCommandHandler(
    IAssessmentTaskRepository repository,
    ILockedTaskLookup lockedTaskLookup,
    IUnitOfWork unitOfWork
) : ICommandHandler<UpdateHomeworkDescriptionCommand>
{
    public async Task<Result> Handle(UpdateHomeworkDescriptionCommand request, CancellationToken ct)
    {
        var (homeworkId, tutorId, description) = request;
        var homework = await repository.GetHomeworkAsync(homeworkId, ct);
        if (homework is null) return Result.Failure(TaskErrors.NotFound);
        if (homework.TutorId != tutorId) return Result.Failure(TaskErrors.Forbidden);
        bool isLocked = await lockedTaskLookup.IsLockedAsync(homework.Id, ct);
        if (isLocked) return Result.Failure(TaskErrors.Locked);
        homework.UpdateDescription(description);
        await unitOfWork.CommitAsync(ct);
        return Result.Success();
    }
}
