using LiveLearn.BuildingBlocks;

namespace LiveLearn.Enrollment.Domain.DomainEvents;


public sealed record EnrollmentActivatedDomainEvent(
    Guid EnrollmentId,
    Guid StudentId,
    Guid CourseId,
    long Version,
    long CourseRevision
) : IDomainEvent;
