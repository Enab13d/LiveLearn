using LiveLearn.Enrollment.Domain.Entities;
using LiveLearn.Enrollment.Infrastructure.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Enrollment.Infrastructure.Contexts;


public sealed class WriteDbContext(DbContextOptions<WriteDbContext> options) : DbContext(options)
{
    public DbSet<CourseEnrollment> CourseEnrollments { get; set; } = null!;

    public DbSet<CourseStructureItem> CourseStructureItems { get; set; } = null!;

    public DbSet<CourseRevision> CourseRevisions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly,
            a => a.AssemblyQualifiedName?.Contains("EntityConfigurations.Write") == true);

        modelBuilder.AddTransactionalOutboxEntities();
    }
}
