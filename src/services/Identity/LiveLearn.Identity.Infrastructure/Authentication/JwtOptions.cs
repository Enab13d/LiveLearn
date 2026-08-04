namespace LiveLearn.Identity.Infrastructure.Authentication;

public class JwtOptions
{

    public string? Authority { get; set; }
    public string? ValidIssuer { get; set; }
    public string? ValidAudience { get; set; }
    public string? MetadataAddress { get; set; }
    public string? AuthorizationUrl { get; set; }
    public string? TokenUrl { get; set; }
}

