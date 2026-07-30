using System.Security.Cryptography;
using System.Text;
using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Queries;

public sealed record GetCatalogQuery(

    int PageNumber,
    int PageSize,
    Guid? CategoryId = null,
    Guid? TutorId = null,
    decimal? MaxPrice = null,
    string? Query = null
) : IQuery<PagedResult<CatalogItemDto>>, ICacheableQuery
{
    public bool BypassCache => false;

    public string CacheKey => $"catalog:{PageNumber}:{ComputeHash($"{PageSize}:{CategoryId}:{TutorId}:{MaxPrice}:{Query}")}".ToLower();

    private static string ComputeHash(string input) =>
        Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(input)))[..16];

    public CacheEntryOptions? Options => new()
    {
        Expiration = TimeSpan.FromMinutes(20),
        LocalCacheExpiration = TimeSpan.FromMinutes(5)
    };

    public IEnumerable<string> Tags => ["catalog"];


}
