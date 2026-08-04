namespace LiveLearn.BuildingBlocks;

public sealed class PagedResult<T>(IReadOnlyList<T> items, int totalCount, int currentPage, int pageSize)
{
    public IReadOnlyList<T> Items { get; init; } = items;
    public int CurrentPage { get; init; } = currentPage;
    public int PageSize { get; init; } = pageSize;
    public int TotalCount { get; init; } = totalCount;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
};
