using LiveLearn.BuildingBlocks;
using LiveLearn.Assessment.Application.Authorization;
using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Application.Services;
using LiveLearn.Assessment.Infrastructure.Configuration;
using LiveLearn.Assessment.Infrastructure.Contexts;
using LiveLearn.Assessment.Infrastructure.Messaging;
using LiveLearn.Assessment.Infrastructure.Repositories;
using LiveLearn.Assessment.Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LiveLearn.Assessment.Infrastructure.Messaging.Consumers;

namespace LiveLearn.Assessment.Infrastructure;


public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddAssessmentInfrastructure(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
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
                policy.RequireRole(nameof(Role.Admin)))
            .AddPolicy(AuthorizationPolicies.TutorPolicy, policy =>
                policy.RequireRole(nameof(Role.Tutor)))
            .AddPolicy(AuthorizationPolicies.StudentPolicy, policy =>
                policy.RequireRole(nameof(Role.Student)));
        services.Configure<RabbitMQOptions>(configuration.GetSection(RabbitMQOptions.SectionName));

        var rabbitMQOptions = configuration.GetRequiredSection(RabbitMQOptions.SectionName).Get<RabbitMQOptions>()
            ?? throw new InvalidOperationException("RabbitMQOptions options not defined");

        services.AddScoped<IEventBus, MassTransitEventBus>();
        services.AddMassTransit(x =>
        {

            x.AddConsumer<CourseArchivedConsumer, CourseArchivedConsumerDefinition>();
            x.AddConsumer<CoursePublishedConsumer, CoursePublishedConsumerDefinition>();
            x.AddConsumer<TaskAssignedConsumer>();
            x.AddConsumer<TaskRemovedFromSectionConsumer>();
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

        var connectionString = configuration.GetConnectionString("AssessmentDb")
            ?? throw new InvalidOperationException("Connection string 'AssessmentDb' is not configured.");

        services.AddDbContext<WriteDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDbContext<ReadDbContext>(options =>
            options.UseNpgsql(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddScoped<IAssessmentTaskRepository, AssessmentTaskRepository>();
        services.AddScoped<IHomeworkSubmissionRepository, HomeworkSubmissionRepository>();
        services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();
        services.AddScoped<ILockedTaskLookup, LockedTaskLookup>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();



        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjectionExtensions).Assembly);
            cfg.AddOpenBehavior(typeof(CacheInvalidationBehavior<,>));
        });

        services.AddHealthChecks().AddNpgSql(connectionString);

        return services;
    }
}
