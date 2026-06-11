namespace LiveLearn.Gateway.Infrastructure.Authentication;

internal class KeycloakOptions
{

    public const string SectionName = "Keycloak";
    public string Authority => $"{Host}/realms/{Realm}";
    public string ValidIssuer { get; set; } = string.Empty;

    public string Realm { get; set; } = string.Empty;

    public string Host { get; set; } = string.Empty;

    public string ManagementUrl {get;set;} = string.Empty;

    public string ValidAudience { get; set; } = string.Empty;
}

