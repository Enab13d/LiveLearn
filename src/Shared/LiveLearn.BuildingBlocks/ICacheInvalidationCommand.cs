namespace LiveLearn.BuildingBlocks;

public interface ICacheInvalidationCommand
{
    IEnumerable<string> Tags { get; }
}
