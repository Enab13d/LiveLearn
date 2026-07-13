namespace LiveLearn.BuildingBlocks;


public interface ICacheService
{
    Task<T?> GetAsync<T>(string cacheKey, CancellationToken ct = default);

    Task SetAsync<T>(string cacheKey, T data, CacheEntryOptions? options = default, CancellationToken ct = default);

    Task RemoveAsync(string cacheKey, CancellationToken ct = default);
}
