namespace LiveLearn.BuildingBlocks;

public interface ICachableQuery
{
    bool BypassCache { get; }
    string CacheKey { get; }

    TimeSpan? SlidingExpiration { get; }

    TimeSpan? AbsoluteExpiration { get; }
}
