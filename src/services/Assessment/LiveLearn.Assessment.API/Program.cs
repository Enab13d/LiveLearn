using System.Reflection;
using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
using LiveLearn.Assessment.API.ExceptionHandlers;
using LiveLearn.Assessment.Application;
using LiveLearn.Assessment.Infrastructure;
using LiveLearn.Assessment.Infrastructure.Configuration;
using LiveLearn.Assessment.Infrastructure.Contexts;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    var serviceName = builder.Environment.ApplicationName;
    var serviceVersion = typeof(Program).Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";

    var serviceInstanceId = Environment.MachineName;

    builder.Host.UseSerilog((context, services, config) =>
    {
        config
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Service", builder.Environment.ApplicationName)
            .WriteTo.OpenTelemetry(options =>
            {
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = serviceName,
                    ["service.version"] = serviceVersion,
                    ["service.instance.id"] = serviceInstanceId
                };
            });

        if (context.HostingEnvironment.IsDevelopment())
        {
            config.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] {SourceContext}: {Message:lj}{NewLine}{Exception}");
        }
        else
        {
            config.WriteTo.Console(new CompactJsonFormatter());
        }
    });

    builder.Services.AddOpenTelemetry()
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource("Yarp.ReverseProxy"))
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddMeter("System.Runtime"))
        .ConfigureResource(cfg =>
        {
            cfg.AddService(serviceName, serviceVersion: serviceVersion, serviceInstanceId: serviceInstanceId);
        })
        .UseOtlpExporter();

    var jwtOptions = builder.Configuration.GetRequiredSection(nameof(JwtOptions)).Get<JwtOptions>()
       ?? throw new InvalidOperationException("JWT options not defined");

    builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, context, ct) =>
        {
            document.Servers = [new OpenApiServer { Url = "/assessment" }];
            var scheme = new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(jwtOptions.AuthorizationUrl!),
                        TokenUrl = new Uri(jwtOptions.TokenUrl!),
                        Scopes = new Dictionary<string, string>
                        {
                             { "openid", "OpenID Connect" }
                        }
                    }
                }
            };
            document.Components ??= new OpenApiComponents();
            var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["OAuth2"] = scheme
            };
            document.Components.SecuritySchemes = securitySchemes;
            return Task.CompletedTask;
        });
    });
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.Configure<RouteOptions>(options =>
    {
        options.LowercaseUrls = true;
    });
    builder.Services.AddControllers()
        .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    builder.Services.AddAssessmentApplication();
    builder.Services.AddAssessmentInfrastructure(builder.Configuration, builder.Environment.IsDevelopment());


    var app = builder.Build();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();

        using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        await db.Database.MigrateAsync();
    }

    app.UseExceptionHandler();
    app.UseStatusCodePages();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSerilogRequestLogging();
    app.MapControllers();
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });


    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
