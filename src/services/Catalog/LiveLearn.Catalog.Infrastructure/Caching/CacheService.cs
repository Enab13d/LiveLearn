using LiveLearn.BuildingBlocks;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace LiveLearn.Catalog.Infrastructure.Caching;


internal sealed class CacheService(HybridCache cache, IOptions<HybridCacheOptions> hybridCacheOptions) : ICacheService
{

    public async Task<T> GetOrCreateAsync<T>(
        string cacheKey,
        Func<CancellationToken, ValueTask<T>> factory,
        CacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken ct = default
    )
    {
        var defaults = hybridCacheOptions.Value.DefaultEntryOptions;

        var entryOptions = options is null
            ? null
            : new HybridCacheEntryOptions
            {
                Expiration = options.Expiration ?? defaults?.Expiration,
                LocalCacheExpiration = options.LocalCacheExpiration ?? defaults?.LocalCacheExpiration,
            };

        return await cache.GetOrCreateAsync(
            cacheKey,
            async cancel => await factory(cancel), 
            entryOptions,
            tags,
            ct
        );
    }

    public async Task RemoveAsync(string cacheKey, CancellationToken ct = default)
    {
        await cache.RemoveAsync(cacheKey, ct);
    }

    public async Task RemoveByTagAsync(string tag, CancellationToken ct = default)
    {
        await cache.RemoveByTagAsync(tag, ct);
    }
}
