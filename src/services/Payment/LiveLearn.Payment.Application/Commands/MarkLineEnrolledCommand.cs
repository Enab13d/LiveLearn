using LiveLearn.BuildingBlocks;

namespace LiveLearn.Payment.Application.Commands;


// Sent by OrderLineEnrolledConsumer; unlocks the student's refund request for the line (D6).
public sealed record MarkLineEnrolledCommand(Guid OrderId, Guid OrderLineId) : ICommand;
