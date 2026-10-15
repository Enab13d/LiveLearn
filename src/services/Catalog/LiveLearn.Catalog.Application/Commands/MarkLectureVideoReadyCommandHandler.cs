using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;


internal sealed class MarkLectureVideoReadyCommandHandler(ICourseRepository repository, IUnitOfWork unitOfWork) : ICommandHandler<MarkLectureVideoReadyCommand>
{
    public async Task<Result> Handle(MarkLectureVideoReadyCommand request, CancellationToken ct)
    {
        var course = await repository.GetByIdAsync(request.CourseId, ct);

        if (course is null) return Result.Failure(CourseErrors.NotFound);

        var result = course.MarkLectureVideoReady(request.LectureId, request.VideoId, request.Duration);

        if (!result.IsSuccess) return result;

        await unitOfWork.CommitAsync(ct);

        return result;
    }
}
