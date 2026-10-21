using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Domain.DomainEvents;


public sealed record TaskCompletedDomainEvent(
    Guid StudentId,
    Guid TaskId,
    Guid CourseId
) : IDomainEvent;
