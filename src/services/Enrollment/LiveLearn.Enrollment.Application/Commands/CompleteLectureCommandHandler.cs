using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Application.Repositories;
using LiveLearn.Enrollment.Application.Services;
using LiveLearn.Enrollment.Domain.Enums;
using LiveLearn.Enrollment.Domain.Errors;

namespace LiveLearn.Enrollment.Application.Commands;

internal sealed class CompleteLectureCommandHandler(
    IEnrollmentRepository enrollmentRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<CompleteLectureCommand>
{
    public async Task<Result> Handle(CompleteLectureCommand request, CancellationToken ct)
    {
        // Looked up by the caller's id, so another student's enrollment is never reachable.
        var enrollment = await enrollmentRepository.GetByStudentAndCourseAsync(currentUser.UserId, request.CourseId, ct);
        if (enrollment is null) return Result.Failure(EnrollmentErrors.NotFound);

        var result = enrollment.CompleteItem(request.LectureId, EnrollmentItemKind.Lecture, timeProvider.GetUtcNow());
        if (!result.IsSuccess) return result;

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
