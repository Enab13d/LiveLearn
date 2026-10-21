using LiveLearn.BuildingBlocks;

namespace LiveLearn.Enrollment.Domain.DomainEvents;


public sealed record EnrollmentRevokedDomainEvent(
    Guid EnrollmentId,
    Guid StudentId,
    Guid CourseId,
    long Version,
    long CourseRevision,
    string Reason
) : IDomainEvent;
