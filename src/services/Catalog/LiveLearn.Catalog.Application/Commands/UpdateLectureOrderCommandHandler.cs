using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;

internal sealed class UpdateLectureOrderCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateLectureOrderCommand>
{
    public async Task<Result> Handle(UpdateLectureOrderCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);

        if (course is null)
            return Result.Failure(CourseErrors.NotFound);

        if (course.TutorId != request.TutorId)
            return Result.Failure(CourseErrors.Forbidden);

        var result = course.UpdateLectureOrder(request.SectionId, request.LectureId, request.Order);

        if (!result.IsSuccess) return Result.Failure(result.FirstError);

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
