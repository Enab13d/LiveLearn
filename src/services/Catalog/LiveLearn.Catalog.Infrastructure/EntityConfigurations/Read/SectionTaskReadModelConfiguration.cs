using LiveLearn.Catalog.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Catalog.Infrastructure.EntityConfigurations.Read;


internal sealed class SectionTaskReadModelConfiguration : IEntityTypeConfiguration<SectionTaskReadModel>
{
    public void Configure(EntityTypeBuilder<SectionTaskReadModel> builder)
    {
        builder.ToTable("section_tasks");
        builder.HasKey(e => e.TaskId);
    }
}
