namespace LiveLearn.Ordering.Application.Messages;

// Ordering-internal: starts the purchase saga (O8). Published through the outbox in the placement transaction.
public sealed record OrderPlaced(Guid OrderId, Guid StudentId, DateTimeOffset OccurredOn);
