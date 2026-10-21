using LiveLearn.BuildingBlocks;

namespace LiveLearn.Enrollment.Domain.DomainEvents;


public sealed record CourseCompletedDomainEvent(Guid EnrollmentId, Guid CourseId, Guid StudentId) : IDomainEvent;
