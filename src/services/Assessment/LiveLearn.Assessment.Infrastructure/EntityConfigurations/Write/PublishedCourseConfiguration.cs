using LiveLearn.Assessment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Assessment.Infrastructure.EntityConfigurations.Write;


internal sealed class PublishedCourseConfiguration : IEntityTypeConfiguration<PublishedCourse>
{
    public void Configure(EntityTypeBuilder<PublishedCourse> builder)
    {
        builder.ToTable("published_courses");

        builder.HasKey(e => e.CourseId);
        builder.Property(e => e.CourseId).ValueGeneratedNever();
    }
}
