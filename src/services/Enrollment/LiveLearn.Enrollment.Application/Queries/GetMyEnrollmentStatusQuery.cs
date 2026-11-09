using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Application.Dto;

namespace LiveLearn.Enrollment.Application.Queries;

// Polled by the frontend after checkout; not found until EnrollStudent from the purchase saga is applied.
public sealed record GetMyEnrollmentStatusQuery(Guid CourseId) : IQuery<EnrollmentStatusDto>;
