using LiveLearn.Catalog.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Catalog.Infrastructure.EntityConfigurations.Write;


internal sealed class CourseEnrollmentConfiguration : IEntityTypeConfiguration<CourseEnrollment>
{
    public void Configure(EntityTypeBuilder<CourseEnrollment> builder)
    {
        builder.ToTable("course_enrollments");

        builder.HasKey(e => new { e.StudentId, e.CourseId });

        builder.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(16);

        builder.Property(e => e.CourseRevision).IsRequired();

        builder.Property(e => e.Version).IsRequired().IsConcurrencyToken();

        builder.Property(e => e.UpdatedAt).IsRequired();
    }
}
