using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Queries;

public sealed record GetCoursesByTutorQuery(
    Guid TutorId, 
    int PageNumber, 
    int PageSize
) : IQuery<PagedResult<CourseDto>>;
