using Microsoft.EntityFrameworkCore;
using LiveLearn.Identity.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace LiveLearn.Identity.Infrastructure.Context;

internal sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options, IConfiguration configuration) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = configuration.GetConnectionString("UsersDb") ??
        throw new InvalidOperationException("Users database connection string is not configured");
        optionsBuilder.UseNpgsql(connectionString);
        base.OnConfiguring(optionsBuilder);
    }
}
