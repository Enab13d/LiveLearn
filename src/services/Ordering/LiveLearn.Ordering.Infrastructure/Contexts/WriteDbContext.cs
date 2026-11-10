using LiveLearn.Ordering.Domain.Entities;
using LiveLearn.Ordering.Infrastructure.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Ordering.Infrastructure.Contexts;


public sealed class WriteDbContext(DbContextOptions<WriteDbContext> options) : DbContext(options)
{
    public DbSet<CourseProjection> CourseProjections { get; set; } = null!;
    public DbSet<EnrollmentProjection> EnrollmentProjections { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly,
            a => a.AssemblyQualifiedName?.Contains("EntityConfigurations.Write") == true);

        modelBuilder.AddTransactionalOutboxEntities();
    }
}
