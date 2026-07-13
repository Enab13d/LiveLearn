using LiveLearn.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Catalog.Infrastructure.EntityConfigurations.Write;


internal sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("courses");
        builder.Property(e => e.Title).IsRequired().HasMaxLength(128);
        builder.Property(e => e.Description).IsRequired().HasMaxLength(2048);
        builder.Property(e => e.ThumbnailUrl).IsRequired(false).HasMaxLength(2048);
        builder.Property(e => e.Price).HasPrecision(18, 2);
        builder.Property(e => e.Status).HasConversion<string>();
        builder.HasMany(e => e.Sections).WithOne().HasForeignKey(e => e.CourseId);

        builder.HasOne<Category>().WithMany().HasForeignKey(e => e.CategoryId);


    }

}


