using LiveLearn.Ordering.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Ordering.Infrastructure.EntityConfigurations.Read;


internal sealed class CourseProjectionReadModelConfiguration : IEntityTypeConfiguration<CourseProjectionReadModel>
{
    public void Configure(EntityTypeBuilder<CourseProjectionReadModel> builder)
    {
        builder.ToTable("course_projections");
        builder.HasKey(e => e.CourseId);
        builder.Property(e => e.Price).HasPrecision(19, 4);
        builder.Property(e => e.Status).HasConversion<string>();
    }
}
