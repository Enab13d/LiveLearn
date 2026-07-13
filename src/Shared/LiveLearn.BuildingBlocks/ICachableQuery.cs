namespace LiveLearn.BuildingBlocks;

public interface ICachableQuery
{
    bool BypassCache { get; }
    string CacheKey { get; }

    CacheEntryOptions? Options {get;}
}
