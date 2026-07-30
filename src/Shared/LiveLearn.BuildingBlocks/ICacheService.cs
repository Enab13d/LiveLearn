namespace LiveLearn.BuildingBlocks;


public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(
        string cacheKey,
        Func<CancellationToken, ValueTask<T>> factory,
        CacheEntryOptions? options = default,
        IEnumerable<string>? tags = null,
        CancellationToken ct = default
        );

    Task RemoveByTagAsync(string tag, CancellationToken ct = default);
    Task RemoveAsync(string cacheKey, CancellationToken ct = default);
}
