using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Infrastructure.Contexts;
using MediatR;

namespace LiveLearn.Catalog.Infrastructure;


internal sealed class DomainEventDispatchBehavior<TRequest, TResponse>(
    WriteDbContext dbContext,
    IPublisher publisher)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var response = await next(ct);
        if (response is Result result && !result.IsSuccess) return response;

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
