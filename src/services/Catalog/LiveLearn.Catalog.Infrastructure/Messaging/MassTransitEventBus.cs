using LiveLearn.BuildingBlocks;
using MassTransit;

namespace LiveLearn.Catalog.Infrastructure.Messaging;

internal sealed class MassTransitEventBus(IPublishEndpoint publishEndpoint) : IEventBus
{
    public async Task PublishAsync<T>(T message, CancellationToken ct = default) where T : class
    {
        await publishEndpoint.Publish<T>(message, ct);
    }
}
