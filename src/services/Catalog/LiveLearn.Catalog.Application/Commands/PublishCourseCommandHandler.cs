using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Domain.Errors;
using LiveLearn.Contracts.Catalog;

namespace LiveLearn.Catalog.Application.Commands;

internal sealed class PublishCourseCommandHandler(
    ICourseRepository courseRepository, 
    IUnitOfWork unitOfWork, 
    IEventBus eventBus, 
    TimeProvider timeProvider
    ) : ICommandHandler<PublishCourseCommand>
{
    public async Task<Result> Handle(PublishCourseCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);

        if (course is null) return Result.Failure(CourseErrors.NotFound);

        if (course.TutorId != request.TutorId) return Result.Failure(CourseErrors.Forbidden);

        var result = course.Publish();

        if (!result.IsSuccess) return result;

        await eventBus.PublishAsync<CoursePublishedEvent>(
            new(course.Id, course.TutorId, course.CategoryId, course.Title, course.Price, timeProvider.GetUtcNow()), ct);

        await unitOfWork.CommitAsync(ct);

        return Result.Success();

    }
}
