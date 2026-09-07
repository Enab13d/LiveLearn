using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands.Handlers;


internal sealed class UnassignTaskCommandHandler(
    IAssessmentTaskRepository repository,
    IUnitOfWork unitOfWork
) : ICommandHandler<UnassignTaskCommand>
{
    public async Task<Result> Handle(UnassignTaskCommand request, CancellationToken ct)
    {
        var task = await repository.GetByIdAsync(request.TaskId, ct);
        if (task is null) return Result.Failure(TaskErrors.NotFound);
        task.Unassign();
        await unitOfWork.CommitAsync(ct);
        return Result.Success();
    }
}
