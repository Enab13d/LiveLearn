using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;


internal sealed class AssignTaskToSectionCommandHandler(ICourseRepository courseRepository, ITaskReferenceLookup taskReferenceLookup, IUnitOfWork unitOfWork) : ICommandHandler<AssignTaskToSectionCommand>
{
    public async Task<Result> Handle(AssignTaskToSectionCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);
        if (course is null) return Result.Failure(CourseErrors.NotFound);
        if (course.TutorId != request.TutorId) return Result.Failure(CourseErrors.Forbidden);

        var taskReference = await taskReferenceLookup.GetByTaskIdAsync(request.TaskId, ct);
        if (taskReference is null) return Result.Failure(SectionErrors.TaskNotFound);

        var result = course.AssignTaskToSection(request.SectionId, request.TaskId, taskReference.TaskType);
        if (!result.IsSuccess) return Result.Failure(result.FirstError);
        
        await unitOfWork.CommitAsync(ct);

        return result;


    }
}
