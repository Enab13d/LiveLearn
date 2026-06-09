namespace LiveLearn.Gateway.Infrastructure.Authentication;

internal class JwtOptions
{
    public string Authority { get; set; } = string.Empty;
    public string ValidIssuer { get; set; } = string.Empty;

    public string ValidAudience { get; set; } = string.Empty;
}

