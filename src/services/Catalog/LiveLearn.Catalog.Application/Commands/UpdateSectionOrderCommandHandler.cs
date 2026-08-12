using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Domain.Common;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;

internal sealed class UpdateSectionOrderCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateSectionOrderCommand, OrderUpdateResult>
{
    public async Task<Result<OrderUpdateResult>> Handle(UpdateSectionOrderCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);

        if (course is null)
            return Result<OrderUpdateResult>.Failure(CourseErrors.NotFound);

        if (course.TutorId != request.TutorId)
            return Result<OrderUpdateResult>.Failure(CourseErrors.Forbidden);

        var result = course.UpdateSectionOrder(request.SectionId, request.PreviousSectionId, request.NextSectionId);

        if (!result.IsSuccess) return Result<OrderUpdateResult>.Failure(result.FirstError);

        await unitOfWork.CommitAsync(ct);

        return result;
    }
}
