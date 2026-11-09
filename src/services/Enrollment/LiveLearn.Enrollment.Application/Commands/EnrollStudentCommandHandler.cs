using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Application.Repositories;
using LiveLearn.Enrollment.Application.Services;
using LiveLearn.Enrollment.Domain.Entities;
using LiveLearn.Enrollment.Domain.Enums;
using LiveLearn.Enrollment.Domain.Errors;

namespace LiveLearn.Enrollment.Application.Commands;

internal sealed class EnrollStudentCommandHandler(
    IEnrollmentRepository enrollmentRepository,
    ICourseStructureLookup courseStructure,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<EnrollStudentCommand>
{
    public async Task<Result> Handle(EnrollStudentCommand request, CancellationToken ct)
    {
        var enrollment = await enrollmentRepository.GetByStudentAndCourseAsync(request.StudentId, request.CourseId, ct);

        // Redelivery: this line already granted access. Any status, so a late EnrollStudent never
        // re-grants a line that has since been refunded.
        if (enrollment is not null && enrollment.OrderLineId == request.OrderLineId)
            return Result.Success();

        // Access through another line (or an admin/legacy path): the saga refunds this line.
        if (enrollment is not null && enrollment.Status != EnrollmentStatus.Revoked)
            return Result.Failure(EnrollmentErrors.AlreadyEnrolled);

        // CoursePublishedEvent hasn't reached the structure projection yet.
        var snapshot = await courseStructure.GetSnapshotAsync(request.CourseId, ct);
        if (snapshot is null)
            return Result.Failure(EnrollmentErrors.CourseStructureUnavailable);

        var now = timeProvider.GetUtcNow();

        if (enrollment is null)
            await enrollmentRepository.AddAsync(
                CourseEnrollment.Create(Guid.NewGuid(), request.StudentId, request.CourseId, snapshot, now,
                    request.OrderId, request.OrderLineId), ct);
        else
            // Repurchase after a refund through another line.
            enrollment.Reactivate(snapshot, now, request.OrderId, request.OrderLineId);

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
