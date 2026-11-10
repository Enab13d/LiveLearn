using LiveLearn.BuildingBlocks;

namespace LiveLearn.Payment.Domain.DomainEvents;


public sealed record OrderPaymentExpiredDomainEvent(Guid OrderPaymentId, Guid OrderId, DateTimeOffset ExpiredAt) : IDomainEvent;
