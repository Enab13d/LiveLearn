using MediatR;

namespace LiveLearn.BuildingBlocks;

public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
where TDomainEvent : IDomainEvent
{ }
