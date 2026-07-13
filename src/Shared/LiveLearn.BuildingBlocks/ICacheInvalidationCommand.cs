namespace LiveLearn.BuildingBlocks;

public interface ICacheInvalidationCommand
{
    IEnumerable<string> CacheKeys { get; }
}
