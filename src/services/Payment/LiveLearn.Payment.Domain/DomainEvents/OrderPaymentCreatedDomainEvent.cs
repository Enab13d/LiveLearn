using LiveLearn.BuildingBlocks;

namespace LiveLearn.Payment.Domain.DomainEvents;


public sealed record OrderPaymentCreatedDomainEvent(Guid OrderPaymentId, Guid OrderId) : IDomainEvent;
