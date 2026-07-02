using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.DomainEvents;


public sealed record LectureAddedDomainEvent(Guid CourseId, Guid SectionId, Guid LectureId) : IDomainEvent;
