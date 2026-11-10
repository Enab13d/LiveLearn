using LiveLearn.BuildingBlocks;

namespace LiveLearn.Payment.Domain.DomainEvents;


public sealed record OrderPaymentPaidDomainEvent(Guid OrderPaymentId, Guid OrderId, IReadOnlyCollection<Guid> OrderLineIds, DateTimeOffset PaidAt) : IDomainEvent;
