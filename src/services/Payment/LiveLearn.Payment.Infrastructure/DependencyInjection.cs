using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Application.Configuration;
using LiveLearn.Payment.Application.Repositories;
using LiveLearn.Payment.Application.Services;
using LiveLearn.Payment.Infrastructure.Configuration;
using LiveLearn.Payment.Infrastructure.Contexts;
using LiveLearn.Payment.Infrastructure.Messaging;
using LiveLearn.Payment.Infrastructure.Messaging.Consumers;
using LiveLearn.Payment.Infrastructure.Repositories;
using LiveLearn.Payment.Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Stripe;

namespace LiveLearn.Payment.Infrastructure;


public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddPaymentInfrastructure(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
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
            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("payment", false));

            x.AddConsumer<CreateCheckoutSessionConsumer, CreateCheckoutSessionConsumerDefinition>();
            x.AddConsumer<RequestPaymentConsumer, RequestPaymentConsumerDefinition>();

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

        var connectionString = configuration.GetConnectionString("PaymentDb")
            ?? throw new InvalidOperationException("Connection string 'PaymentDb' is not configured.");

        services.AddDbContext<WriteDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDbContext<ReadDbContext>(options =>
            options.UseNpgsql(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOrderPaymentRepository, OrderPaymentRepository>();

        // Both sections are optional — the options carry the D5 / D6 defaults.
        services.AddOptions<RevenueShareOptions>()
            .Bind(configuration.GetSection(RevenueShareOptions.SectionName))
            .Validate(o => o.TutorShareRate is > 0 and < 1, "RevenueShare:TutorShareRate must be between 0 and 1")
            .ValidateOnStart();
        services.AddOptions<RefundOptions>()
            .Bind(configuration.GetSection(RefundOptions.SectionName))
            .Validate(o => o.WindowDays > 0, "Refund:WindowDays must be positive")
            .ValidateOnStart();

        services.AddOptions<StripeOptions>()
            .Bind(configuration.GetSection(StripeOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.SecretKey), "Stripe:SecretKey is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.PlatformWebhookSecret), "Stripe:PlatformWebhookSecret is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.ConnectWebhookSecret), "Stripe:ConnectWebhookSecret is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.ThinWebhookSecret), "Stripe:ThinWebhookSecret is required")
            .Validate(o => Uri.IsWellFormedUriString(o.CheckoutSuccessUrl, UriKind.Absolute), "Stripe:CheckoutSuccessUrl must be an absolute URL")
            .Validate(o => Uri.IsWellFormedUriString(o.CheckoutCancelUrl, UriKind.Absolute), "Stripe:CheckoutCancelUrl must be an absolute URL")
            .ValidateOnStart();
        // StripeClient is thread-safe and reuses its HTTP connections, so one instance serves the app (D9).
        services.AddSingleton(sp => new StripeClient(sp.GetRequiredService<IOptions<StripeOptions>>().Value.SecretKey));
        services.AddScoped<ICheckoutGateway, StripeCheckoutGateway>();
        services.AddScoped<IRefundGateway, StripeRefundGateway>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjectionExtensions).Assembly));

        // RabbitMQ is covered by MassTransit's own bus health check, registered by AddMassTransit.
        services.AddHealthChecks()
            .AddNpgSql(connectionString);

        return services;
    }
}
