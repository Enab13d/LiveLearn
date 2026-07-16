using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;

internal sealed class AddLectureCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork) : ICommandHandler<AddLectureCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddLectureCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);

        if (course is null) return Result<Guid>.Failure(CourseErrors.NotFound);

        if (course.TutorId != request.TutorId) return Result<Guid>.Failure(CourseErrors.Forbidden);

        var lectureId = Guid.NewGuid();

        var result = course.AddLecture(
            request.SectionId,
            lectureId,
            request.Title,
            request.LectureType,
            request.Order,
            request.Duration,
            request.Description
        );

        if (!result.IsSuccess) return Result<Guid>.Failure(result.FirstError);

        await unitOfWork.CommitAsync(ct);

        return Result<Guid>.Success(lectureId);

    }
}
