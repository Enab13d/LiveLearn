using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;

internal sealed class UpdateLectureCommandHandler(ICurrentUser currentUser, ICourseRepository courseRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateLectureCommand>
{
    public async Task<Result> Handle(UpdateLectureCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);

        if (course is null)
            return Result.Failure(CourseErrors.NotFound);

        if (course.TutorId != currentUser.UserId)
            return Result.Failure(CourseErrors.Forbidden);

        var result = course.UpdateLecture(request.SectionId, request.LectureId, request.Title, request.Description);

        if (!result.IsSuccess) return Result.Failure(result.FirstError);

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
