using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;


internal sealed class ArchiveLectureCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork) : ICommandHandler<ArchiveLectureCommand>
{
    public async Task<Result> Handle(ArchiveLectureCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);
        if (course is null) return Result.Failure(CourseErrors.NotFound);
        if (course.TutorId != request.TutorId) return Result.Failure(CourseErrors.Forbidden);

        var result = course.ArchiveLecture(request.LectureId);
        if (!result.IsSuccess) return result;

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
