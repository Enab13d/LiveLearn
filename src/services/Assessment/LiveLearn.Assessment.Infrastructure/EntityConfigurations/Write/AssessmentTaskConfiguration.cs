using LiveLearn.Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Assessment.Infrastructure.EntityConfigurations.Write;



internal sealed class AssessmentTaskConfiguration : IEntityTypeConfiguration<AssessmentTask>
{
    public void Configure(EntityTypeBuilder<AssessmentTask> builder)
    {
        builder.ToTable("tasks");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(128);
        builder.UseTptMappingStrategy();
    }
}
