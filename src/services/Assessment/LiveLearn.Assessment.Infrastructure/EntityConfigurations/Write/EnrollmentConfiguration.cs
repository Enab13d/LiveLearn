using LiveLearn.Assessment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Assessment.Infrastructure.EntityConfigurations.Write;


internal sealed class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("enrollments");

        builder.HasKey(e => new { e.StudentId, e.CourseId });

        builder.Property(e => e.Status).IsRequired().HasConversion<string>();

        builder.Property(e => e.UpdatedAt);

    }
}
