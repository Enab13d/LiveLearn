using LiveLearn.Ordering.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Ordering.Infrastructure.EntityConfigurations.Read;


internal sealed class EnrollmentProjectionReadModelConfiguration : IEntityTypeConfiguration<EnrollmentProjectionReadModel>
{
    public void Configure(EntityTypeBuilder<EnrollmentProjectionReadModel> builder)
    {
        builder.ToTable("enrollment_projections");
        builder.HasKey(e => new { e.StudentId, e.CourseId });
    }
}
