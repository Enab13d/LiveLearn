using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;


internal sealed class AddSectionCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork) : ICommandHandler<AddSectionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddSectionCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);

        if (course is null) return Result<Guid>.Failure(CourseErrors.NotFound);

        if (course.TutorId != request.TutorId) return Result<Guid>.Failure(CourseErrors.Forbidden);

        var result = course.AddSection(request.SectionId, request.Title, request.Order);

        if (!result.IsSuccess) return Result<Guid>.Failure(result.FirstError);

        await unitOfWork.CommitAsync(ct);

        return Result<Guid>.Success(request.SectionId);


    }
}
