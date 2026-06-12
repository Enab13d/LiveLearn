using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Infrastructure.Context;
using MediatR;

namespace LiveLearn.Identity.Infrastructure;


internal sealed class DomainEventDispatchBehavior<TRequest, TResponse>(
    IdentityDbContext dbContext,
    IPublisher publisher) 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var response = await next(ct);

        var domainEvents = dbContext.ChangeTracker
                            .Entries<AggregateRoot<Guid>>()
                            .SelectMany(e => e.Entity.DomainEvents)
                            .ToList();

        foreach (var domainEvent in domainEvents)
            await publisher.Publish(domainEvent, ct);

        dbContext.ChangeTracker
            .Entries<AggregateRoot<Guid>>()
            .ToList()
            .ForEach(e => e.Entity.ClearDomainEvents());
        
        return response;


    }
}
