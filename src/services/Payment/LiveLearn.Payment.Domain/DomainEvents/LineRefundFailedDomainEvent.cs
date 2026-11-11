using LiveLearn.BuildingBlocks;

namespace LiveLearn.Payment.Domain.DomainEvents;


// WasConfirmed: the line had been Refunded (OrderLineRefunded already published) before Stripe reported the failure.
public sealed record LineRefundFailedDomainEvent(Guid OrderPaymentId, Guid OrderId, Guid OrderLineId, Guid CourseId, Guid StudentId, bool WasConfirmed, DateTimeOffset FailedAt) : IDomainEvent;
