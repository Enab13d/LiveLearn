using LiveLearn.BuildingBlocks;

namespace LiveLearn.Payment.Domain.DomainEvents;


public sealed record OrderPaymentRejectedDomainEvent(Guid OrderPaymentId, Guid OrderId, string Reason, DateTimeOffset RejectedAt) : IDomainEvent;
