using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Application.Dto;

namespace LiveLearn.Enrollment.Application.Queries;

// Polled by the frontend after checkout; not found until PaymentConfirmedEvent is applied.
public sealed record GetMyEnrollmentStatusQuery(Guid CourseId) : IQuery<EnrollmentStatusDto>;
