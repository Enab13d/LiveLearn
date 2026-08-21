using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;

internal sealed class PublishCourseCommandHandler(
    ICourseRepository courseRepository, 
    IUnitOfWork unitOfWork, 
    TimeProvider timeProvider
    ) : ICommandHandler<PublishCourseCommand>
{
    public async Task<Result> Handle(PublishCourseCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);

        if (course is null) return Result.Failure(CourseErrors.NotFound);

        if (course.TutorId != request.TutorId) return Result.Failure(CourseErrors.Forbidden);

        var result = course.Publish(timeProvider.GetUtcNow());

        if (!result.IsSuccess) return result;

        await unitOfWork.CommitAsync(ct);

        return Result.Success();

    }
}
