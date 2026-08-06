using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;

internal sealed class DeleteSectionCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork) : ICommandHandler<DeleteSectionCommand>
{
    public async Task<Result> Handle(DeleteSectionCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);

        if (course is null)
            return Result.Failure(CourseErrors.NotFound);

        if (course.TutorId != request.TutorId)
            return Result.Failure(CourseErrors.Forbidden);

        var result = course.DeleteSection(request.SectionId);

        if (!result.IsSuccess) return Result.Failure(result.FirstError);

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
