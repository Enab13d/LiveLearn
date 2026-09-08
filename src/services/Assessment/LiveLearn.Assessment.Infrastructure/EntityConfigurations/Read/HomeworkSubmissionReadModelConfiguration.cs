using LiveLearn.Assessment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Assessment.Infrastructure.EntityConfigurations.Read;


internal sealed class HomeworkSubmissionReadModelConfiguration : IEntityTypeConfiguration<HomeworkSubmissionReadModel>
{
    public void Configure(EntityTypeBuilder<HomeworkSubmissionReadModel> builder)
    {
        builder.ToTable("homework_submissions");
    }
}
