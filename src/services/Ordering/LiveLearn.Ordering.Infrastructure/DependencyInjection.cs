using LiveLearn.Ordering.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LiveLearn.Ordering.Infrastructure;


public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddOrderingInfrastructure(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        services.AddSingleton(TimeProvider.System);
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
        var jwtOptions = configuration.GetRequiredSection(nameof(JwtOptions)).Get<JwtOptions>()
            ?? throw new InvalidOperationException("JWT options not defined");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = jwtOptions.Authority;
            options.MetadataAddress = jwtOptions.MetadataAddress ?? throw new InvalidOperationException("Metadata address not specified");
            options.MapInboundClaims = false;
            options.RequireHttpsMetadata = !isDevelopment;
            options.TokenValidationParameters = new()
            {
                RoleClaimType = "role",
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.ValidIssuer,
                ValidAudience = jwtOptions.ValidAudience,
            };
        });
        services.AddAuthorization();

        var connectionString = configuration.GetConnectionString("OrderingDb")
            ?? throw new InvalidOperationException("Connection string 'OrderingDb' is not configured.");

        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Connection string 'Redis' is not configured.");

        // RabbitMQ is covered by MassTransit's own bus health check once AddMassTransit is registered.
        services.AddHealthChecks().AddNpgSql(connectionString).AddRedis(redisConnectionString);

        return services;
    }
}
