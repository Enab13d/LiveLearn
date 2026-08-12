using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Domain.Common;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;

internal sealed class UpdateLectureOrderCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateLectureOrderCommand, OrderUpdateResult>
{
    public async Task<Result<OrderUpdateResult>> Handle(UpdateLectureOrderCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);

        if (course is null)
            return Result<OrderUpdateResult>.Failure(CourseErrors.NotFound);

        if (course.TutorId != request.TutorId)
            return Result<OrderUpdateResult>.Failure(CourseErrors.Forbidden);

        var result = course.UpdateLectureOrder(request.SectionId, request.LectureId, request.PreviousLectureId, request.NextLectureId);

        if (!result.IsSuccess) return Result<OrderUpdateResult>.Failure(result.FirstError);

        await unitOfWork.CommitAsync(ct);

        return result;
    }
}
