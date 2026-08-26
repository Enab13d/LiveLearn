using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Domain.DomainEvents;


public sealed record TaskFailedDomainEvent(
    Guid TaskId, 
    Guid StudentId, 
    Guid SectionId,
    Guid CourseId
) : IDomainEvent;
