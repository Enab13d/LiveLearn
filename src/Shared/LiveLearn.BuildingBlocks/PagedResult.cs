namespace LiveLearn.BuildingBlocks;

public sealed class PagedResult<T>(List<T> items, int count, int pageNumber, int pageSize)
{
    public IReadOnlyList<T> Items { get; init; } = items;
    public int CurrentPage { get; init; } = pageNumber;
    public int PageSize { get; init; } = pageSize;
    public int TotalCount { get; init; } = count;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
};
