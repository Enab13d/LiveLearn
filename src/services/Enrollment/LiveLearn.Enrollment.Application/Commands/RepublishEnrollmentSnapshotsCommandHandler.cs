using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Enrollment;
using LiveLearn.Enrollment.Application.Repositories;

namespace LiveLearn.Enrollment.Application.Commands;

internal sealed class RepublishEnrollmentSnapshotsCommandHandler(
    IEnrollmentRepository enrollmentRepository,
    IEventBus eventBus,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<RepublishEnrollmentSnapshotsCommand, int>
{
    public async Task<Result<int>> Handle(RepublishEnrollmentSnapshotsCommand request, CancellationToken ct)
    {
        var enrollments = await enrollmentRepository.GetAllAsync(ct);
        var occurredOn = timeProvider.GetUtcNow();

        foreach (var enrollment in enrollments)
        {
            await eventBus.PublishAsync(new EnrollmentSnapshotEvent(
                enrollment.StudentId,
                enrollment.CourseId,
                enrollment.Status.ToString(),
                enrollment.CourseRevision,
                enrollment.Version,
                occurredOn), ct);
        }

        // The publishes are staged in the EF outbox and only persist with this commit.
        await unitOfWork.CommitAsync(ct);

        return enrollments.Count;
    }
}
