using System.Text.Json;
using LiveLearn.BuildingBlocks;
using Microsoft.Extensions.Caching.Distributed;

namespace LiveLearn.Catalog.Infrastructure.Caching;


internal sealed class CacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string cacheKey, CancellationToken ct = default)
    {
        var cachedItem = await cache.GetAsync(cacheKey, ct);
        if (cachedItem is null) return default;
        return JsonSerializer.Deserialize<T>(cachedItem);
    }

    public async Task RemoveAsync(string cacheKey, CancellationToken ct = default)
    {
        await cache.RemoveAsync(cacheKey, ct);
    }

    public async Task SetAsync<T>(string cacheKey, T data, CacheEntryOptions? options = null, CancellationToken ct = default)
    {
        var serializedEntry = JsonSerializer.SerializeToUtf8Bytes(data);
        await cache.SetAsync(cacheKey, serializedEntry, new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = options?.AbsoluteExpiration,
            SlidingExpiration = options?.SlidingExpiration

        }, ct);
    }
}
