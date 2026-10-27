using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Application.Dto;

namespace LiveLearn.Enrollment.Application.Queries;

// The caller's enrollments, all statuses.
public sealed record GetStudentEnrollmentsQuery(int PageNumber, int PageSize) : IQuery<PagedResult<EnrollmentDto>>;
