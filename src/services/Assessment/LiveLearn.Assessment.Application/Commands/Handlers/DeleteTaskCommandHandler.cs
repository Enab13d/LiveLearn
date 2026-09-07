using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Application.Services;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands.Handlers;


internal sealed class DeleteTaskCommandHandler(
    IAssessmentTaskRepository repository,
    ILockedTaskLookup lockedTaskLookup,
    IUnitOfWork unitOfWork
) : ICommandHandler<DeleteTaskCommand>
{
    public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken ct)
    {
        var task = await repository.GetByIdAsync(request.TaskId, ct);
        if (task is null) return Result.Failure(TaskErrors.NotFound);
        if (task.TutorId != request.TutorId) return Result.Failure(TaskErrors.Forbidden);
        bool isLocked = await lockedTaskLookup.IsLockedAsync(task.Id, ct);
        if (isLocked) return Result.Failure(TaskErrors.Locked);
        repository.Delete(task);
        task.MarkAsDeleted();
        await unitOfWork.CommitAsync(ct);
        return Result.Success();
    }
}
