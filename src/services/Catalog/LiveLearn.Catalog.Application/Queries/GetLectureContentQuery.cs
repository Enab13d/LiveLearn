using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Queries;


public sealed record GetLectureContentQuery(Guid CourseId, Guid SectionId, Guid LectureId) : IQuery<LectureContentDto>;
