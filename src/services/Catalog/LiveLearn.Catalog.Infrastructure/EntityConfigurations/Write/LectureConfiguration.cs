using LiveLearn.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Catalog.Infrastructure.EntityConfigurations.Write;


internal sealed class LectureConfiguration : IEntityTypeConfiguration<Lecture>
{
    public void Configure(EntityTypeBuilder<Lecture> builder)
    {
        builder.ToTable("lectures");
        builder.Property(e => e.Title).IsRequired().HasMaxLength(128);
        builder.Property(e => e.Type).IsRequired().HasConversion<string>();
        builder.HasIndex(e => new { e.SectionId, e.Order }).IsUnique();
        builder.Property(e => e.Description).IsRequired();
        builder.Property(e => e.VideoUrl).HasMaxLength(2048);
    }
}
