
using LiveLearn.Catalog.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Catalog.Infrastructure.EntityConfigurations.Read;


internal sealed class SectionReadModelConfiguration : IEntityTypeConfiguration<SectionReadModel>
{

    public void Configure(EntityTypeBuilder<SectionReadModel> builder)
    {
        builder.ToTable("sections");
        builder.HasKey(e => e.Id);
        builder.HasMany(e => e.Lectures).WithOne().HasForeignKey(e => e.SectionId);

    }
}


