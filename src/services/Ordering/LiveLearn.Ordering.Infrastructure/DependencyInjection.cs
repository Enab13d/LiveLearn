using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Application.Configuration;
using LiveLearn.Ordering.Application.Repositories;
using LiveLearn.Ordering.Application.Services;
using LiveLearn.Ordering.Infrastructure.Configuration;
using LiveLearn.Ordering.Infrastructure.Contexts;
using LiveLearn.Ordering.Infrastructure.Messaging;
using LiveLearn.Ordering.Infrastructure.Messaging.Consumers;
using LiveLearn.Ordering.Infrastructure.Repositories;
using LiveLearn.Ordering.Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

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

        services.Configure<RabbitMQOptions>(configuration.GetSection(RabbitMQOptions.SectionName));

        var rabbitMQOptions = configuration.GetRequiredSection(RabbitMQOptions.SectionName).Get<RabbitMQOptions>()
            ?? throw new InvalidOperationException("RabbitMQOptions options not defined");

        services.AddScoped<IEventBus, MassTransitEventBus>();
        services.AddMassTransit(x =>
        {
            x.AddConsumer<CourseArchivedConsumer, CourseArchivedConsumerDefinition>();
            x.AddConsumer<CoursePriceChangedConsumer, CoursePriceChangedConsumerDefinition>();
            x.AddConsumer<CoursePublishedConsumer, CoursePublishedConsumerDefinition>();
            x.AddConsumer<EnrollmentActivatedConsumer, EnrollmentActivatedConsumerDefinition>();
            x.AddConsumer<EnrollmentRevokedConsumer, EnrollmentRevokedConsumerDefinition>();
            x.AddConsumer<OrderLineRefundedConsumer, OrderLineRefundedConsumerDefinition>();
            x.AddConsumer<OrderLineRefundFailedConsumer, OrderLineRefundFailedConsumerDefinition>();
            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("ordering", false));
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

        var connectionString = configuration.GetConnectionString("OrderingDb")
            ?? throw new InvalidOperationException("Connection string 'OrderingDb' is not configured.");

        services.AddDbContext<WriteDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDbContext<ReadDbContext>(options =>
            options.UseNpgsql(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICourseProjectionLookup, CourseProjectionLookup>();
        services.AddScoped<IEnrollmentLookup, EnrollmentLookup>();
        services.AddScoped<IActiveOrderLineLookup, ActiveOrderLineLookup>();

        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Connection string 'Redis' is not configured.");

        // AbortOnConnectFail = false: the service starts and keeps reconnecting while Redis is down —
        // only the cart depends on it, and losing a cart is acceptable (O3).
        var redisOptions = ConfigurationOptions.Parse(redisConnectionString);
        redisOptions.AbortOnConnectFail = false;
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisOptions));

        // The section is optional — CartOptions carries the O3 defaults.
        services.AddOptions<CartOptions>()
            .Bind(configuration.GetSection(CartOptions.SectionName))
            .Validate(o => o.MaxItems > 0, "Cart:MaxItems must be positive")
            .Validate(o => o.Ttl > TimeSpan.Zero, "Cart:Ttl must be positive")
            .ValidateOnStart();
        services.AddScoped<ICartStore, RedisCartStore>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjectionExtensions).Assembly));

        // RabbitMQ is covered by MassTransit's own bus health check, registered by AddMassTransit.
        services.AddHealthChecks()
            .AddNpgSql(connectionString)
            .AddRedis(sp => sp.GetRequiredService<IConnectionMultiplexer>());

        return services;
    }
}
