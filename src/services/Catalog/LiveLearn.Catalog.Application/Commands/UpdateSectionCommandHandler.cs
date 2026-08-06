using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;

internal sealed class UpdateSectionCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateSectionCommand>
{
    public async Task<Result> Handle(UpdateSectionCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);

        if (course is null)
            return Result.Failure(CourseErrors.NotFound);

        if (course.TutorId != request.TutorId)
            return Result.Failure(CourseErrors.Forbidden);

        var result = course.UpdateSection(request.SectionId, request.Title);

        if (!result.IsSuccess) return Result.Failure(result.FirstError);

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
