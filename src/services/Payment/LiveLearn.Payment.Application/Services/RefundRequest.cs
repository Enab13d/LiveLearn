using LiveLearn.Payment.Domain.Common;

namespace LiveLearn.Payment.Application.Services;


// OrderId / OrderLineId go into the refund's metadata, so its webhooks can be traced back to the line (C1).
public sealed record RefundRequest(string ChargeId, Money Amount, Guid OrderId, Guid OrderLineId);
