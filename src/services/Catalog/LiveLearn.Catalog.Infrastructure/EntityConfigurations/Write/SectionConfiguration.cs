using LiveLearn.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Catalog.Infrastructure.EntityConfigurations.Write;


internal sealed class SectionConfiguration : IEntityTypeConfiguration<Section>
{

    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("sections");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Title).IsRequired().HasMaxLength(128);
        builder.HasMany(e => e.Lectures).WithOne().HasForeignKey(e => e.SectionId);
        builder.HasMany(e => e.SectionTasks)
            .WithOne()
            .HasForeignKey(e => e.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}


