using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Application.Authorization;
using LiveLearn.Enrollment.Application.Repositories;
using LiveLearn.Enrollment.Application.Services;
using LiveLearn.Enrollment.Infrastructure.Configuration;
using LiveLearn.Enrollment.Infrastructure.Contexts;
using LiveLearn.Enrollment.Infrastructure.Messaging;
using LiveLearn.Enrollment.Infrastructure.Messaging.Consumers;
using LiveLearn.Enrollment.Infrastructure.Repositories;
using LiveLearn.Enrollment.Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LiveLearn.Enrollment.Infrastructure;


public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddEnrollmentInfrastructure(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
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
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.AdminPolicy, policy =>
                policy.RequireRole("Admin"));

        services.Configure<RabbitMQOptions>(configuration.GetSection(RabbitMQOptions.SectionName));

        var rabbitMQOptions = configuration.GetRequiredSection(RabbitMQOptions.SectionName).Get<RabbitMQOptions>()
            ?? throw new InvalidOperationException("RabbitMQOptions options not defined");

        services.AddScoped<IEventBus, MassTransitEventBus>();
        services.AddMassTransit(x =>
        {
            x.AddConsumer<CoursePublishedConsumer, CoursePublishedConsumerDefinition>();
            x.AddConsumer<CourseStructureRevisedConsumer, CourseStructureRevisedConsumerDefinition>();
            x.AddConsumer<PaymentConfirmedConsumer, PaymentConfirmedConsumerDefinition>();
            x.AddConsumer<PaymentFailedConsumer, PaymentFailedConsumerDefinition>();
            x.AddConsumer<RefundCompletedConsumer, RefundCompletedConsumerDefinition>();
            x.AddConsumer<TaskCompletedConsumer, TaskCompletedConsumerDefinition>();
            x.SetKebabCaseEndpointNameFormatter();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMQOptions.Host, rabbitMQOptions.VirtualHost, h =>
                {
                    h.Username(rabbitMQOptions.Username);
                    h.Password(rabbitMQOptions.Password);
                });
                cfg.ConfigureEndpoints(context);
            });

            x.AddEntityFrameworkOutbox<WriteDbContext>(o =>
            {
                o.UsePostgres();

                o.UseBusOutbox();
            });
        });

        var connectionString = configuration.GetConnectionString("EnrollmentDb")
            ?? throw new InvalidOperationException("Connection string 'EnrollmentDb' is not configured.");

        services.AddDbContext<WriteDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDbContext<ReadDbContext>(options =>
            options.UseNpgsql(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<ICourseStructureLookup, CourseStructureLookup>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjectionExtensions).Assembly));

        services.AddHealthChecks().AddNpgSql(connectionString);

        return services;
    }
}
