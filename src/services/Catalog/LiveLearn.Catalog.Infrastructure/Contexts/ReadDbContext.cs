using LiveLearn.Catalog.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Contexts;


internal sealed class ReadDbContext(DbContextOptions<ReadDbContext> options) : DbContext(options)
{
    public DbSet<CourseReadModel> Courses { get; set; } = null!;
    public DbSet<SectionReadModel> Sections { get; set; } = null!;
    public DbSet<LectureReadModel> Lectures { get; set; } = null!;
    public DbSet<CategoryReadModel> Categories { get; set; } = null!;
    public DbSet<SectionTaskReadModel> SectionTasks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadDbContext).Assembly,
            a => a.AssemblyQualifiedName?.Contains("EntityConfigurations.Read") == true
        );
    }

}
