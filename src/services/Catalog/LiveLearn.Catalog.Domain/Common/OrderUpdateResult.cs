namespace LiveLearn.Catalog.Domain.Common;

public sealed record OrderUpdateResult(Guid Id, int Order, bool Rebalanced, IReadOnlyDictionary<Guid, int>? Snapshot);
