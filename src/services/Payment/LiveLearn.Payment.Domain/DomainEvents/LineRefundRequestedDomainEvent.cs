using LiveLearn.BuildingBlocks;

namespace LiveLearn.Payment.Domain.DomainEvents;


public sealed record LineRefundRequestedDomainEvent(Guid OrderPaymentId, Guid OrderId, Guid OrderLineId, DateTimeOffset RequestedAt) : IDomainEvent;
