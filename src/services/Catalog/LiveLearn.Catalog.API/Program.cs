using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
using LiveLearn.Catalog.API.ExceptionHandlers;
using LiveLearn.Catalog.Application;
using LiveLearn.Catalog.Infrastructure;
using LiveLearn.Catalog.Infrastructure.Authentication;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, config) =>
    {
        config
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Service", "catalog");
        if (context.HostingEnvironment.IsDevelopment())
        {
            config.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] {SourceContext}: {Message:lj}{NewLine}{Exception}");
        }
        else
        {
            config.WriteTo.Console(new CompactJsonFormatter());
        }
    });

    var jwtOptions = builder.Configuration.GetRequiredSection(nameof(JwtOptions)).Get<JwtOptions>()
       ?? throw new InvalidOperationException("JWT options not defined");
       
    builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, context, ct) =>
        {
            document.Servers = [new OpenApiServer { Url = "/catalog" }];
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

    builder.Services.AddCatalogApplication();
    builder.Services.AddCatalogInfrastructure(builder.Configuration, builder.Environment.IsDevelopment());


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
