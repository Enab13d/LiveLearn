using LiveLearn.Payment.Domain.Common;

namespace LiveLearn.Payment.Application.Services;


// OrderId comes from the session's metadata / client_reference_id: webhooks carry no trace context (C1).
// ChargeId, Fee and PaidAt are set only when IsPaid.
public sealed record CompletedCheckoutSession(Guid OrderId, bool IsPaid, string? ChargeId, Money? Fee, DateTimeOffset? PaidAt);
