using LiveLearn.BuildingBlocks;

namespace LiveLearn.Payment.Domain.DomainEvents;


public sealed record LineRefundedDomainEvent(Guid OrderPaymentId, Guid OrderId, Guid OrderLineId, Guid CourseId, Guid StudentId, DateTimeOffset RefundedAt) : IDomainEvent;
