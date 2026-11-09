using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Application.Repositories;
using LiveLearn.Enrollment.Application.Services;
using LiveLearn.Enrollment.Domain.Enums;
using LiveLearn.Enrollment.Domain.Errors;

namespace LiveLearn.Enrollment.Application.Commands;

internal sealed class RestoreRefundedEnrollmentCommandHandler(
    IEnrollmentRepository enrollmentRepository,
    ICourseStructureLookup courseStructure,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<RestoreRefundedEnrollmentCommand>
{
    public async Task<Result> Handle(RestoreRefundedEnrollmentCommand request, CancellationToken ct)
    {
        var enrollment = await enrollmentRepository.GetByStudentAndCourseAsync(request.StudentId, request.CourseId, ct);
        // E.g. the reverted refund was a compensation refund of a line that never enrolled.
        if (enrollment is null) return Result.Failure(EnrollmentErrors.NotFound);

        // Only the access this line's refund took away comes back; e.g. after a repurchase the enrollment
        // is already active through another line. The consumer acknowledges and logs.
        if (enrollment.Status != EnrollmentStatus.Revoked
            || enrollment.RevocationReason != RevocationReason.Refund
            || enrollment.OrderLineId != request.OrderLineId)
            return Result.Failure(EnrollmentErrors.NotRevokedByRefund);

        var snapshot = await courseStructure.GetSnapshotAsync(request.CourseId, ct);
        if (snapshot is null) return Result.Failure(EnrollmentErrors.CourseStructureUnavailable);

        enrollment.Reactivate(snapshot, timeProvider.GetUtcNow(), request.OrderId, request.OrderLineId);

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
