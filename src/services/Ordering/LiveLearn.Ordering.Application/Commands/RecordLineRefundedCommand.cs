using LiveLearn.BuildingBlocks;

namespace LiveLearn.Ordering.Application.Commands;


// Sent by OrderLineRefundedConsumer: Payment confirmed the refund of this line (O10).
public sealed record RecordLineRefundedCommand(Guid OrderId, Guid OrderLineId) : ICommand;
