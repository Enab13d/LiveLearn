namespace LiveLearn.BuildingBlocks;

public sealed class CacheEntryOptions
{
    public TimeSpan? SlidingExpiration { get; init; }
    public TimeSpan? AbsoluteExpiration { get; init; }
}
