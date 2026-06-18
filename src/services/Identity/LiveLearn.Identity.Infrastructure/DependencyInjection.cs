using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Application.Repositories;
using LiveLearn.Identity.Domain.Entities;
using LiveLearn.Identity.Infrastructure.Authentication;
using LiveLearn.Identity.Application.Authorization;
using LiveLearn.Identity.Infrastructure.Context;
using LiveLearn.Identity.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LiveLearn.Identity.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
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
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.AdminPolicy, policy =>
                policy.RequireRole(nameof(Role.Admin)))
            .AddPolicy(AuthorizationPolicies.TutorPolicy, policy =>
                policy.RequireRole(nameof(Role.Tutor)))
            .AddPolicy(AuthorizationPolicies.StudentPolicy, policy =>
                policy.RequireRole(nameof(Role.Student)));

        var connectionString = configuration.GetConnectionString("UsersDb")
            ?? throw new InvalidOperationException("Connection string 'UsersDb' is not configured.");

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DomainEventDispatchBehavior<,>));

        return services;
    }
}
