using LiveLearn.Assessment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Assessment.Infrastructure.EntityConfigurations.Read;


internal sealed class QuizReadModelConfiguration : IEntityTypeConfiguration<QuizReadModel>
{
    public void Configure(EntityTypeBuilder<QuizReadModel> builder)
    {
        builder.ToTable("tasks");
        builder.Property<string>("TaskType");
        builder.HasQueryFilter(e => EF.Property<string>(e, "TaskType") == "Quiz");
        builder.OwnsMany(e => e.Questions, quesion =>
        {
            quesion.ToJson();
            quesion.OwnsMany(q => q.Answers);
        });
    }
}
