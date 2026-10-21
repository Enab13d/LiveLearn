using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;

internal sealed class MarkLectureVideoUploadedCommandHandler(ICurrentUser currentUser, ICourseRepository courseRepository, IUnitOfWork unitOfWork) : ICommandHandler<MarkLectureVideoUploadedCommand>
{
    public async Task<Result> Handle(MarkLectureVideoUploadedCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);

        if (course is null) return Result.Failure(CourseErrors.NotFound);

        if (course.TutorId != currentUser.UserId) return Result.Failure(CourseErrors.Forbidden);

        var result = course.MarkLectureVideoUploaded(request.LectureId, request.VideoId);

        if (!result.IsSuccess) return Result.Failure(result.FirstError);

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
