namespace LiveLearn.Ordering.API.Dto.QueryParameters;


public sealed class PageableQueryParams
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
