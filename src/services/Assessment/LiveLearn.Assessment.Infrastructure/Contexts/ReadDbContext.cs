using LiveLearn.Assessment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.Contexts;


internal sealed class ReadDbContext(DbContextOptions<ReadDbContext> options) : DbContext(options)
{

    public DbSet<AssessmentTaskReadModel> AssessmentTasks { get; set; } = null!;
    public DbSet<QuizReadModel> Quizzes { get; set; } = null!;

    public DbSet<HomeworkReadModel> Homeworks { get; set; } = null!;

    public DbSet<HomeworkSubmissionReadModel> HomeworkSubmissions { get; set; } = null!;

    public DbSet<QuizAttemptReadModel> QuizAttempts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadDbContext).Assembly,
            a => a.AssemblyQualifiedName?.Contains("EntityConfigurations.Read") == true
        );
    }
}
