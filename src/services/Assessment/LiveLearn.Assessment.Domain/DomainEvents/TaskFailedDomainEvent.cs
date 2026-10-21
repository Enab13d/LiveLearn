using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Domain.DomainEvents;


public sealed record TaskFailedDomainEvent(
    Guid StudentId,
    Guid TaskId,
    Guid CourseId
) : IDomainEvent;
