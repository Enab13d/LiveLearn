using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Ordering.Infrastructure.Contexts;


public sealed class WriteDbContext(DbContextOptions<WriteDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly,
            a => a.AssemblyQualifiedName?.Contains("EntityConfigurations.Write") == true);

        modelBuilder.AddTransactionalOutboxEntities();
    }
}
