using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Enrollment.Infrastructure.Contexts;


// Backs the query handlers; read models are added with the queries.
internal sealed class ReadDbContext(DbContextOptions<ReadDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadDbContext).Assembly,
            a => a.AssemblyQualifiedName?.Contains("EntityConfigurations.Read") == true
        );
    }
}
