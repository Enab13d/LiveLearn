using LiveLearn.BuildingBlocks;

namespace LiveLearn.Payment.Application.Commands;


// ByStudent: sent by the API for the caller's own order (needs an enrolled line); otherwise by
// RefundOrderLineConsumer as the saga's compensation (D6).
public sealed record RequestLineRefundCommand(Guid OrderId, Guid OrderLineId, string Reason, bool ByStudent) : ICommand;
