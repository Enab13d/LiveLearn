using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Infrastructure.Contexts;
using MediatR;

namespace LiveLearn.Payment.Infrastructure;


internal sealed class UnitOfWork(WriteDbContext dbContext, IPublisher publisher) : IUnitOfWork
{
    public async Task<int> CommitAsync(CancellationToken ct = default)
    {
        var domainEvents = dbContext.ChangeTracker
                    .Entries<AggregateRoot<Guid>>()
                    .SelectMany(e => e.Entity.DomainEvents)
                    .ToList();

        // Dispatched before SaveChangesAsync: IEventBus publishes made by domain-event handlers are
        // staged in the EF outbox and persist with the same save.
        foreach (var domainEvent in domainEvents)
            await publisher.Publish(domainEvent, ct);

        dbContext.ChangeTracker
            .Entries<AggregateRoot<Guid>>()
            .ToList()
            .ForEach(e => e.Entity.ClearDomainEvents());

        return await dbContext.SaveChangesAsync(ct);
    }
}
