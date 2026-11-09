using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Domain.Enums;

namespace LiveLearn.Enrollment.Application.Commands;

// Sent by OrderLineRefundedConsumer (Refund) and the admin removal endpoint (AdminRemoval).
// OrderLineId is the refunded order line; a refund revokes only the access that line granted. Ignored for AdminRemoval.
public sealed record RevokeEnrollmentCommand(Guid StudentId, Guid CourseId, RevocationReason Reason, Guid? OrderLineId) : ICommand;
