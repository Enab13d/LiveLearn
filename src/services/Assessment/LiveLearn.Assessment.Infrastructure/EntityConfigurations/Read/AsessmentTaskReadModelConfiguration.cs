using LiveLearn.Assessment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Assessment.Infrastructure.EntityConfigurations.Read;



internal sealed class AssessmentTaskReadModelConfiguration : IEntityTypeConfiguration<AssessmentTaskReadModel>
{
    public void Configure(EntityTypeBuilder<AssessmentTaskReadModel> builder)
    {
        builder.ToTable("tasks");

        builder.HasKey(e => e.Id);

    }
}
