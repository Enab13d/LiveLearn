using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;


internal sealed class ArchiveCourseCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork) : ICommandHandler<ArchiveCourseCommand>
{
    public async Task<Result> Handle(ArchiveCourseCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);
        if (course is null) return Result.Failure(CourseErrors.NotFound);
        if (course.TutorId != request.TutorId) return Result.Failure(CourseErrors.Forbidden);
        var result = course.Archive();

        if (result.IsSuccess) await unitOfWork.CommitAsync(ct);

        return result;
    }
}
