namespace LiveLearn.BuildingBlocks;

public sealed class CacheEntryOptions
{
    public TimeSpan? LocalCacheExpiration { get; init; }
    public TimeSpan? Expiration { get; init; }
}
