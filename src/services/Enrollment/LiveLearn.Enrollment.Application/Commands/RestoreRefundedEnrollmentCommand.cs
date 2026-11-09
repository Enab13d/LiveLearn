using LiveLearn.BuildingBlocks;

namespace LiveLearn.Enrollment.Application.Commands;

// Sent by OrderLineRefundRevertedConsumer: the refund of this order line failed, so its access comes back.
public sealed record RestoreRefundedEnrollmentCommand(
    Guid StudentId,
    Guid CourseId,
    Guid OrderId,
    Guid OrderLineId) : ICommand;
