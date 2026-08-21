using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Infrastructure.Contexts;
using MediatR;

namespace LiveLearn.Catalog.Infrastructure;


internal sealed class UnitOfWork(WriteDbContext dbContext, IPublisher publisher) : IUnitOfWork
{
    public async Task<int> CommitAsync(CancellationToken ct = default)
    {
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

        return await dbContext.SaveChangesAsync(ct);
    }
}
