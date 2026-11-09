using LiveLearn.Ordering.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Ordering.Infrastructure.EntityConfigurations.Write;


internal sealed class CourseProjectionConfiguration : IEntityTypeConfiguration<CourseProjection>
{
    public void Configure(EntityTypeBuilder<CourseProjection> builder)
    {
        builder.ToTable("course_projections");

        builder.HasKey(e => e.CourseId);
        builder.Property(e => e.OwnerId).IsRequired(false);
        builder.Property(e => e.Price).IsRequired(false).HasPrecision(19, 4);
        builder.Property(e => e.PriceUpdatedAt).IsRequired(false);
        builder.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(16);

        // xmin: concurrent applies to the same course (e.g. CoursePublishedEvent and CoursePriceChangedEvent)
        // conflict instead of overwriting each other; the consumer retry re-applies on fresh state.
        builder.Property<uint>("Version").IsRowVersion();
    }
}
