
using LiveLearn.Catalog.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Catalog.Infrastructure.EntityConfigurations.Read;


internal sealed class LectureReadModelConfiguration : IEntityTypeConfiguration<LectureReadModel>
{
    public void Configure(EntityTypeBuilder<LectureReadModel> builder)
    {
        builder.ToTable("lectures");
        builder.HasKey(e => e.Id);
        builder.HasOne<SectionReadModel>().WithMany(e => e.Lectures).HasForeignKey(e => e.SectionId);
        builder.Property(e => e.DurationInSeconds)
            .HasColumnName("Duration")
            .HasConversion(
                v => TimeSpan.FromSeconds(v),
                v => (int)v.TotalSeconds
            );

    }
}
