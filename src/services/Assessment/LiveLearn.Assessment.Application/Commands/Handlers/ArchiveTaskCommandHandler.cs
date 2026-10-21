using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands.Handlers;


// Always allowed, locked or not — enrolled students keep submitting an archived task.
internal sealed class ArchiveTaskCommandHandler(
    IAssessmentTaskRepository repository,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork
) : ICommandHandler<ArchiveTaskCommand>
{
    public async Task<Result> Handle(ArchiveTaskCommand request, CancellationToken ct)
    {
        var task = await repository.GetByIdAsync(request.TaskId, ct);
        if (task is null) return Result.Failure(TaskErrors.NotFound);
        if (task.TutorId != request.TutorId) return Result.Failure(TaskErrors.Forbidden);
        task.Archive(timeProvider.GetUtcNow());
        await unitOfWork.CommitAsync(ct);
        return Result.Success();
    }
}
