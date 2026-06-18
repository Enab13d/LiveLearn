namespace LiveLearn.Gateway.Infrastructure.Authentication;

internal class JwtOptions
{

    public string? Authority { get; set; }
    public string? ValidIssuer { get; set; }
    public string? ValidAudience { get; set; }
    public string? MetadataAddress { get; set; }
}

