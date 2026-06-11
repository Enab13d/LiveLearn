using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LiveLearn.Gateway.Infrastructure.Health;



internal class AuthorizationServerHealthCheck(
    IHttpClientFactory httpClientFactory,
    ILogger<AuthorizationServerHealthCheck> logger) : IHealthCheck
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync(new Uri("http://keycloak:9000/health"), cancellationToken);

            if (!response.IsSuccessStatusCode)
                return HealthCheckResult.Unhealthy("Authorization server is unhealthy");

            var body = await response.Content.ReadFromJsonAsync<HealthCheckResponse>(_jsonOptions, cancellationToken);

            return body?.Status == "UP"
                ? HealthCheckResult.Healthy("Authorization server is healthy")
                : HealthCheckResult.Unhealthy("Authorization server is unhealthy");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Authorization server health check failed");
            return HealthCheckResult.Unhealthy("Authorization server is unhealthy", ex);
        }
    }
}


internal class HealthCheckResponse
{
    public string Status { get; set; } = string.Empty;

    public List<HealthCheck> Checks { get; set; } = [];

}

internal class HealthCheck
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

}


