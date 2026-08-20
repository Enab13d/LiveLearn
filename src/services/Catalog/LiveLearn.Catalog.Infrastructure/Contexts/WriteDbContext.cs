using LiveLearn.Catalog.Domain.Entities;
using LiveLearn.Catalog.Infrastructure.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Contexts;


internal sealed class WriteDbContext(DbContextOptions<WriteDbContext> options) : DbContext(options)
{
    public DbSet<Course> Courses { get; set; } = null!;

    public DbSet<Category> Categories { get; set; } = null!;

    public DbSet<TaskReplica> TaskReplicas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly,
            a => a.AssemblyQualifiedName?.Contains("EntityConfigurations.Write") == true);

        modelBuilder.AddTransactionalOutboxEntities();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSeeding((context, _) =>
        {
            bool exists = context.Set<Category>().Any();
            if (exists) return;
            context.Set<Category>().Add(Category.Create(Guid.NewGuid(), "Web Development", "web-development"));
            context.Set<Category>().Add(Category.Create(Guid.NewGuid(), "Machine Learning", "ml"));
            context.Set<Category>().Add(Category.Create(Guid.NewGuid(), "Data science", "data-science"));
            context.Set<Category>().Add(Category.Create(Guid.NewGuid(), "DevOps", "devops"));
            context.Set<Category>().Add(Category.Create(Guid.NewGuid(), "Design", "design"));
            context.Set<Category>().Add(Category.Create(Guid.NewGuid(), "Quality assurance", "qa"));

            context.SaveChanges();
        });
        optionsBuilder.UseAsyncSeeding(async (context, _, ct) =>
        {
            bool exists = await context.Set<Category>().AnyAsync(ct);
            if (exists) return;
            context.Set<Category>().Add(Category.Create(Guid.NewGuid(), "Web Development", "web-development"));
            context.Set<Category>().Add(Category.Create(Guid.NewGuid(), "Machine Learning", "ml"));
            context.Set<Category>().Add(Category.Create(Guid.NewGuid(), "Data science", "data-science"));
            context.Set<Category>().Add(Category.Create(Guid.NewGuid(), "DevOps", "devops"));
            context.Set<Category>().Add(Category.Create(Guid.NewGuid(), "Design", "design"));
            context.Set<Category>().Add(Category.Create(Guid.NewGuid(), "Quality assurance", "qa"));

            await context.SaveChangesAsync(ct);
        });
    }

}
