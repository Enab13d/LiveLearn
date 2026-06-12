using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Application.Repositories;
using LiveLearn.Identity.Infrastructure.Context;
using LiveLearn.Identity.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LiveLearn.Identity.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("UsersDb")
            ?? throw new InvalidOperationException("Connection string 'UsersDb' is not configured.");

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddMediatR(cfg =>
            cfg.AddOpenBehavior(typeof(DomainEventDispatchBehavior<,>)));

        return services;
    }
}
