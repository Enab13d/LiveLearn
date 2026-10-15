using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Queries;


// RequesterId is null for anonymous callers. The owner also sees archived items (flagged); everyone else sees active items only.
public sealed record GetCourseByIdQuery(Guid CourseId, Guid? RequesterId) : IQuery<CourseDetailDto>;
