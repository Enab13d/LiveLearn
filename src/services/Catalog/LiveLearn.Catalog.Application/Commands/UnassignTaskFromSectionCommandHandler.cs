using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;


internal sealed class UnassignTaskFromSectionCommandHandler(ICurrentUser currentUser, ICourseRepository courseRepository, IUnitOfWork unitOfWork) : ICommandHandler<UnassignTaskFromSectionCommand>
{
    public async Task<Result> Handle(UnassignTaskFromSectionCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);
        if (course is null) return Result.Failure(CourseErrors.NotFound);
        if (course.TutorId != currentUser.UserId) return Result.Failure(CourseErrors.Forbidden);

        var result = course.UnassignTaskFromSection(request.SectionId, request.TaskId);
        if (!result.IsSuccess) return result;

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
