using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;


internal sealed class MarkLectureVideoFailedCommandHandler(ICourseRepository repository, IUnitOfWork unitOfWork) : ICommandHandler<MarkLectureVideoFailedCommand>
{
    public async Task<Result> Handle(MarkLectureVideoFailedCommand request, CancellationToken ct)
    {
        var course = await repository.GetByIdAsync(request.CourseId, ct);

        if (course is null) return Result.Failure(CourseErrors.NotFound);

        var result = course.MarkLectureVideoFailed(request.LectureId, request.VideoId);

        if (!result.IsSuccess) return result;

        await unitOfWork.CommitAsync(ct);

        return result;
    }
}
