using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.DomainEvents;


public sealed record CourseCreatedDomainEvent(Guid Id, Guid TutorId, Guid CategoryId, string Title) : IDomainEvent;
