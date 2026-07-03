using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Domain.Entities;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;

internal sealed class CreateCourseCommandHandler(ICourseRepository courseRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork) : ICommandHandler<CreateCourseCommand, CourseDto>
{
    public async Task<Result<CourseDto>> Handle(CreateCourseCommand request, CancellationToken ct)
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId, ct);
        if(category is null)
            return Result<CourseDto>.Failure(CategoryErrors.NotFound);

        var course = Course.Create(Guid.NewGuid(), request.TutorId, request.CategoryId, request.Title, request.Description, request.Price);
        await courseRepository.AddAsync(course, ct);
        await unitOfWork.CommitAsync(ct);
        CourseDto dto = new(course.Id, course.TutorId, course.CategoryId, course.Title, course.Description, course.ThumbnailUrl, course.Price, course.Status.ToString());
        return dto;
    }

}
