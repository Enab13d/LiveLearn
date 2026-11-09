using LiveLearn.Ordering.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Ordering.Infrastructure.EntityConfigurations.Write;


internal sealed class EnrollmentProjectionConfiguration : IEntityTypeConfiguration<EnrollmentProjection>
{
    public void Configure(EntityTypeBuilder<EnrollmentProjection> builder)
    {
        builder.ToTable("enrollment_projections");

        builder.HasKey(e => new { e.StudentId, e.CourseId });
        builder.Property(e => e.IsActive).IsRequired();

        // Version doubles as the concurrency token: two events for the same enrollment applied concurrently
        // conflict instead of the older one overwriting the newer; the consumer retry re-checks on fresh state.
        builder.Property(e => e.Version).IsRequired().IsConcurrencyToken();
    }
}
