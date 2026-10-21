using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Queries;


public sealed record GetLectureVideoStatusQuery(Guid CourseId, Guid LectureId) : IQuery<string>;
