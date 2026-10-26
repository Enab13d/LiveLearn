using FluentValidation;
using LiveLearn.BuildingBlocks;
using Microsoft.Extensions.DependencyInjection;

namespace LiveLearn.Enrollment.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddEnrollmentApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjectionExtensions).Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjectionExtensions).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        return services;
    }
}
