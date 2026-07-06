using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Domain.Errors;


namespace LiveLearn.Catalog.Application.Commands;


public sealed class UpdateCourseCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateCourseCommand, CourseDto>
{
    public async Task<Result<CourseDto>> Handle(UpdateCourseCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);

        if (course is null)
            return Result<CourseDto>.Failure(CourseErrors.NotFound);

        if (course.TutorId != request.TutorId)
            return Result<CourseDto>.Failure(CourseErrors.Forbidden);

        course.Update(request.Title, request.Description, request.Price, request.CategoryId);

        await unitOfWork.CommitAsync(ct);

        return new CourseDto(
            Id: course.Id,
            TutorId: course.TutorId,
            CategoryId: course.CategoryId,
            Title: course.Title,
            Description: course.Description,
            ThumbnailUrl: course.ThumbnailUrl,
            Price: course.Price,
            Status: course.Status.ToString()
        );
    }
}
