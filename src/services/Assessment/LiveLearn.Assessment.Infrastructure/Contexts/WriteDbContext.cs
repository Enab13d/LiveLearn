using LiveLearn.Assessment.Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.Contexts;


public sealed class WriteDbContext(DbContextOptions<WriteDbContext> options) : DbContext(options)
{
    public DbSet<AssessmentTask> AssessmentTasks { get; set; } = null!;

    public DbSet<Quiz> Quizzes { get; set; } = null!;

    public DbSet<Homework> Homeworks { get; set; } = null!;

    public DbSet<HomeworkSubmission> HomeworkSubmissions { get; set; } = null!;

    public DbSet<QuizAttempt> QuizAttempts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly,
            a => a.AssemblyQualifiedName?.Contains("EntityConfigurations.Write") == true);

        modelBuilder.AddTransactionalOutboxEntities();
    }
}
