using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Infrastructure.Configuration;
using LiveLearn.Catalog.Infrastructure.Contexts;
using LiveLearn.Catalog.Infrastructure.Messaging;
using LiveLearn.Catalog.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LiveLearn.Catalog.Infrastructure;


public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.Configure<RabbitMQOptions>(configuration.GetSection(nameof(RabbitMQOptions)));

        var rabbitMQOptions = configuration.GetRequiredSection(nameof(RabbitMQOptions)).Get<RabbitMQOptions>()
            ?? throw new InvalidOperationException("RabbitMQOptions options not defined");

        services.AddScoped<IEventBus, MassTransitEventBus>();
        services.AddMassTransit(x =>
        {
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

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
