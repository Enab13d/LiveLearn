using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;


internal sealed class ArchiveSectionCommandHandler(ICurrentUser currentUser, ICourseRepository courseRepository, IUnitOfWork unitOfWork) : ICommandHandler<ArchiveSectionCommand>
{
    public async Task<Result> Handle(ArchiveSectionCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);
        if (course is null) return Result.Failure(CourseErrors.NotFound);
        if (course.TutorId != currentUser.UserId) return Result.Failure(CourseErrors.Forbidden);

        var result = course.ArchiveSection(request.SectionId);
        if (!result.IsSuccess) return result;

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
