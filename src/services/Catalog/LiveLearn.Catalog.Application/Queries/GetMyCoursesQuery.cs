using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Queries;


// Courses owned by the caller, all statuses. Not cached — drafts change often and the list is per user.
public sealed record GetMyCoursesQuery(int PageNumber, int PageSize) : IQuery<PagedResult<CourseDto>>;
