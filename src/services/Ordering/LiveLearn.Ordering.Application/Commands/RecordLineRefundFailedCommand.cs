using LiveLearn.BuildingBlocks;

namespace LiveLearn.Ordering.Application.Commands;


// Sent by OrderLineRefundFailedConsumer: Stripe failed a refund that Payment had already confirmed (O6, D6).
public sealed record RecordLineRefundFailedCommand(Guid OrderId, Guid OrderLineId) : ICommand;
