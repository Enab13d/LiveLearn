using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands.Handlers;


internal sealed class AssignTaskToCourseAndSectionCommandHandler(
    IAssessmentTaskRepository repository,
    IUnitOfWork unitOfWork
) : ICommandHandler<AssignTaskToCourseAndSectionCommand>
{
    public async Task<Result> Handle(AssignTaskToCourseAndSectionCommand request, CancellationToken ct)
    {
        var task = await repository.GetByIdAsync(request.TaskId, ct);
        if (task is null) return Result.Failure(TaskErrors.NotFound);
        task.AssignToCourseAndSection(request.CourseId, request.SectionId);
        await unitOfWork.CommitAsync(ct);
        return Result.Success();

    }
}
