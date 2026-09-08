namespace LiveLearn.Assessment.API.Dto.QueryParameters;


public class PageableQueryParams
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
