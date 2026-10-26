using LiveLearn.Enrollment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Enrollment.Infrastructure.EntityConfigurations.Write;


internal sealed class CourseStructureItemConfiguration : IEntityTypeConfiguration<CourseStructureItem>
{
    public void Configure(EntityTypeBuilder<CourseStructureItem> builder)
    {
        builder.ToTable("course_structure_items");

        builder.HasKey(e => new { e.CourseId, e.ItemId });
        builder.Property(e => e.Kind).IsRequired().HasConversion<string>();
    }
}
