using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.DomainEvents;


public sealed record LectureArchivedDomainEvent(Guid CourseId, Guid LectureId, long Revision) : IDomainEvent;
