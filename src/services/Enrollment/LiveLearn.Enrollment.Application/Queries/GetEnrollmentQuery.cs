using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Application.Dto;

namespace LiveLearn.Enrollment.Application.Queries;

// Own enrollment only: another student's enrollment returns not found.
public sealed record GetEnrollmentQuery(Guid EnrollmentId) : IQuery<EnrollmentDetailDto>;
