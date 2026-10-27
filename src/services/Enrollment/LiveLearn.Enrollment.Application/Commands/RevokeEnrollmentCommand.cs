using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Domain.Enums;

namespace LiveLearn.Enrollment.Application.Commands;

// Sent by RefundCompletedConsumer (Refund) and the admin removal endpoint (AdminRemoval).
public sealed record RevokeEnrollmentCommand(Guid StudentId, Guid CourseId, RevocationReason Reason) : ICommand;
