namespace LiveLearn.Identity.Infrastructure.Authentication;

internal class JwtOptions
{

    public string? Authority { get; set; }
    public string? ValidIssuer { get; set; }
    public string? ValidAudience { get; set; }
}

