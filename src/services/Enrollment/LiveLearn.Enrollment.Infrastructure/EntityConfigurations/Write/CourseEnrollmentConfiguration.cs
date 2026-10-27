using LiveLearn.Enrollment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Enrollment.Infrastructure.EntityConfigurations.Write;


internal sealed class CourseEnrollmentConfiguration : IEntityTypeConfiguration<CourseEnrollment>
{
    public void Configure(EntityTypeBuilder<CourseEnrollment> builder)
    {
        builder.ToTable("course_enrollments");

        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(16);

        builder.Property(e => e.RevocationReason).IsRequired(false).HasConversion<string>().HasMaxLength(16);

        // Concurrent revoke/reactivate conflict instead of publishing two events with the same Version.
        builder.Property(e => e.Version).IsRequired().IsConcurrencyToken();

        builder.Property(e => e.CourseRevision).IsRequired();

        builder.Property(e => e.EnrolledAt).IsRequired();

        builder.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

        builder.HasMany(e => e.Items)
            .WithOne()
            .HasForeignKey(e => e.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
