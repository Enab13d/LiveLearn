using LiveLearn.Enrollment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Enrollment.Infrastructure.Contexts;


internal sealed class ReadDbContext(DbContextOptions<ReadDbContext> options) : DbContext(options)
{
    public DbSet<EnrollmentReadModel> Enrollments { get; set; } = null!;
    public DbSet<EnrollmentItemReadModel> EnrollmentItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadDbContext).Assembly,
            a => a.AssemblyQualifiedName?.Contains("EntityConfigurations.Read") == true
        );
    }
}
