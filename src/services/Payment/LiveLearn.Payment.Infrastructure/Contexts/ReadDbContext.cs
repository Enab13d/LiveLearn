using LiveLearn.Payment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Payment.Infrastructure.Contexts;


internal sealed class ReadDbContext(DbContextOptions<ReadDbContext> options) : DbContext(options)
{
    public DbSet<OrderPaymentReadModel> OrderPayments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadDbContext).Assembly,
            a => a.AssemblyQualifiedName?.Contains("EntityConfigurations.Read") == true
        );
    }
}
