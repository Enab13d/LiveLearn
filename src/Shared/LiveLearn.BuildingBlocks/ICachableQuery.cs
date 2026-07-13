namespace LiveLearn.BuildingBlocks;

public interface ICacheableQuery
{
    bool BypassCache { get; }
    string CacheKey { get; }

    CacheEntryOptions? Options {get;}
}
