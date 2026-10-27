using LiveLearn.Enrollment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Enrollment.Infrastructure.EntityConfigurations.Read;

internal sealed class EnrollmentReadModelConfiguration : IEntityTypeConfiguration<EnrollmentReadModel>
{
    public void Configure(EntityTypeBuilder<EnrollmentReadModel> builder)
    {
        builder.ToTable("course_enrollments");
        builder.HasKey(e => e.Id);
        builder.HasMany(e => e.Items).WithOne().HasForeignKey(e => e.EnrollmentId);
    }
}
