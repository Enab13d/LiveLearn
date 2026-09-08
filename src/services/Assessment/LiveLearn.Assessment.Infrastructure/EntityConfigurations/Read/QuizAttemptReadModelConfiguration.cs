using LiveLearn.Assessment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Assessment.Infrastructure.EntityConfigurations.Read;


internal sealed class QuizAttemptReadModelConfiguration : IEntityTypeConfiguration<QuizAttemptReadModel>
{
    public void Configure(EntityTypeBuilder<QuizAttemptReadModel> builder)
    {
        builder.ToTable("quiz_attempts");
    }
}
