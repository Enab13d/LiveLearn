using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Application.Repositories;
using LiveLearn.Enrollment.Domain.Errors;

namespace LiveLearn.Enrollment.Application.Commands;

internal sealed class RevokeEnrollmentCommandHandler(
    IEnrollmentRepository enrollmentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RevokeEnrollmentCommand>
{
    public async Task<Result> Handle(RevokeEnrollmentCommand request, CancellationToken ct)
    {
        var enrollment = await enrollmentRepository.GetByStudentAndCourseAsync(request.StudentId, request.CourseId, ct);
        if (enrollment is null) return Result.Failure(EnrollmentErrors.NotFound);

        // Already revoked: a redelivered refund or a repeated admin removal, no-op in the aggregate.
        // No order line yet: a null OrderLineId matches enrollments created without one (all current ones).
        enrollment.Revoke(request.Reason, null);

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
