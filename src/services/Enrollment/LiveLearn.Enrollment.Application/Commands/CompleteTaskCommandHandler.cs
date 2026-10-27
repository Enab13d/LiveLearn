using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Application.Repositories;
using LiveLearn.Enrollment.Domain.Enums;

namespace LiveLearn.Enrollment.Application.Commands;

internal sealed class CompleteTaskCommandHandler(
    IEnrollmentRepository enrollmentRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<CompleteTaskCommand>
{
    public async Task<Result> Handle(CompleteTaskCommand request, CancellationToken ct)
    {
        var enrollment = await enrollmentRepository.GetByStudentAndCourseAsync(request.StudentId, request.CourseId, ct);

        // A pass by someone not enrolled in this course has nothing to update.
        if (enrollment is null)
            return Result.Success();

        var result = enrollment.CompleteItem(request.TaskId, EnrollmentItemKind.Task, timeProvider.GetUtcNow());
        if (!result.IsSuccess) return result;

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
