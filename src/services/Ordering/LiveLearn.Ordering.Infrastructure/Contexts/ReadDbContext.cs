using LiveLearn.Ordering.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Ordering.Infrastructure.Contexts;


internal sealed class ReadDbContext(DbContextOptions<ReadDbContext> options) : DbContext(options)
{
    public DbSet<OrderReadModel> Orders { get; set; } = null!;
    public DbSet<OrderLineReadModel> OrderLines { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadDbContext).Assembly,
            a => a.AssemblyQualifiedName?.Contains("EntityConfigurations.Read") == true
        );
    }
}
