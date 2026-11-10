using LiveLearn.Payment.Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Payment.Infrastructure.Contexts;


public sealed class WriteDbContext(DbContextOptions<WriteDbContext> options) : DbContext(options)
{
    public DbSet<OrderPayment> OrderPayments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly,
            a => a.AssemblyQualifiedName?.Contains("EntityConfigurations.Write") == true);

        modelBuilder.AddTransactionalOutboxEntities();
    }
}
