using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Application.Repositories;
using LiveLearn.Enrollment.Application.Services;
using LiveLearn.Enrollment.Domain.Entities;
using LiveLearn.Enrollment.Domain.Enums;
using LiveLearn.Enrollment.Domain.Errors;

namespace LiveLearn.Enrollment.Application.Commands;

internal sealed class CreateEnrollmentCommandHandler(
    IEnrollmentRepository enrollmentRepository,
    ICourseStructureLookup courseStructure,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<CreateEnrollmentCommand>
{
    public async Task<Result> Handle(CreateEnrollmentCommand request, CancellationToken ct)
    {
        var enrollment = await enrollmentRepository.GetByStudentAndCourseAsync(request.StudentId, request.CourseId, ct);

        // Active/Completed: a redelivered PaymentConfirmedEvent.
        if (enrollment is not null && enrollment.Status != EnrollmentStatus.Revoked)
            return Result.Success();

        // Transient: the consumer retries until CoursePublishedEvent reaches the structure projection.
        var snapshot = await courseStructure.GetSnapshotAsync(request.CourseId, ct);
        if (snapshot is null)
            return Result.Failure(EnrollmentErrors.CourseStructureUnavailable);

        var now = timeProvider.GetUtcNow();

        if (enrollment is null)
            await enrollmentRepository.AddAsync(
                CourseEnrollment.Create(Guid.NewGuid(), request.StudentId, request.CourseId, snapshot, now), ct);
        else
            enrollment.Reactivate(snapshot, now);

        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
