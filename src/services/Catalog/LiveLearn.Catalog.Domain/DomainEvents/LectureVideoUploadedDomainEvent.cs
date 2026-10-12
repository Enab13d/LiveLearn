using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.DomainEvents;


public sealed record LectureVideoUploadedDomainEvent(Guid CourseId, Guid LectureId, Guid VideoId) : IDomainEvent;
