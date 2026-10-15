using FluentValidation;
using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LiveLearn.Catalog.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjectionExtensions).Assembly);
        services.AddScoped<ICourseContentAccess, CourseContentAccess>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjectionExtensions).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        return services;
    }
}
