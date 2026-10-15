using Amazon.Runtime;
using Amazon.S3;
using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Authorization;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Infrastructure.Authentication;
using LiveLearn.Catalog.Infrastructure.Caching;
using LiveLearn.Catalog.Infrastructure.Configuration;
using LiveLearn.Catalog.Infrastructure.Contexts;
using LiveLearn.Catalog.Infrastructure.Messaging;
using LiveLearn.Catalog.Infrastructure.Messaging.Consumers;
using LiveLearn.Catalog.Infrastructure.Repositories;
using LiveLearn.Catalog.Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LiveLearn.Catalog.Infrastructure;


public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
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
            x.AddConsumer<TaskCreatedConsumer>();
            x.AddConsumer<TaskArchivedConsumer>();
            x.AddConsumer<EnrollmentActivatedConsumer, EnrollmentActivatedConsumerDefinition>();
            x.AddConsumer<EnrollmentRevokedConsumer, EnrollmentRevokedConsumerDefinition>();
            x.AddConsumer<VideoTranscodingCompletedConsumer, VideoTranscodingCompletedConsumerDefinition>();
            x.AddConsumer<VideoTranscodingFailedConsumer, VideoTranscodingFailedConsumerDefinition>();
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

        var connectionString = configuration.GetConnectionString("CatalogDb")
            ?? throw new InvalidOperationException("Connection string 'CatalogDb' is not configured.");

        services.AddDbContext<WriteDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDbContext<ReadDbContext>(options =>
            options.UseNpgsql(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));
        var storageOptions = configuration.GetRequiredSection(StorageOptions.SectionName).Get<StorageOptions>()
            ?? throw new InvalidOperationException("StorageOptions options not defined");

        services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(new AmazonS3Config
        {
            ServiceURL = storageOptions.ServiceUrl,
            ForcePathStyle = true,
            AuthenticationRegion = "us-east-1",
            RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
            ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED,
        }));

        services.AddOptions<WatchTicketOptions>()
            .Bind(configuration.GetRequiredSection(WatchTicketOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.PrivateKey), "WatchTicket:PrivateKey is not configured")
            .ValidateOnStart();
        services.AddSingleton<IWatchTicketSigner, JwtWatchTicketSigner>();

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITaskReferenceLookup, TaskReferenceLookup>();
        services.AddScoped<IEnrollmentLookup, EnrollmentLookup>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IStorageService, MinioStorageService>();

        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10),
                LocalCacheExpiration = TimeSpan.FromMinutes(5)
            };
        });
        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Connection string 'Redis' is not configured.");

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
        });

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjectionExtensions).Assembly);
            cfg.AddOpenBehavior(typeof(CacheInvalidationBehavior<,>));
        });

        services.AddHealthChecks().AddNpgSql(connectionString).AddRedis(redisConnectionString);

        return services;
    }
}
