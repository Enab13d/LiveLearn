namespace LiveLearn.Catalog.Infrastructure.Configuration;


internal sealed class StorageOptions
{
    public static string SectionName => "Storage";

    public string ServiceUrl { get; set; } = string.Empty;

    public string RawVideosBucket { get; set; } = string.Empty;

    public int UploadUrlTtlMinutes { get; set; } = 10;
}
