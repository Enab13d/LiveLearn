using LiveLearn.Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Assessment.Infrastructure.EntityConfigurations.Write;


internal sealed class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
    public void Configure(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.ToTable("quiz_attempts");

        builder.HasKey(e => e.Id);
        builder.HasOne<Quiz>().WithMany().HasForeignKey(e => e.QuizId);
        builder.Property(e => e.Score).IsRequired();
        builder.Property(e => e.AttemptedAt).ValueGeneratedOnAdd().HasDefaultValueSql("now()");
        builder.HasIndex(e => new { e.QuizId, e.StudentId, e.AttemptedAt })
            .IsDescending()
            .HasDatabaseName("IX_QuizAttempts_Quiz_Student_AttemptedAt");
    }
}
