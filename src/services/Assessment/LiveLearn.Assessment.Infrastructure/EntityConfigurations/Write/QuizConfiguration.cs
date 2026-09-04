using LiveLearn.Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Assessment.Infrastructure.EntityConfigurations.Write;

internal sealed class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.ToTable("quizzes");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.PassingScore).IsRequired();

        builder.OwnsMany(e => e.Questions, quesion =>
        {
            quesion.ToJson();
            quesion.OwnsMany(q => q.Answers);
        });
    }
}
