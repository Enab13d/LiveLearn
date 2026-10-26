using LiveLearn.Enrollment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Enrollment.Infrastructure.EntityConfigurations.Write;


internal sealed class CourseRevisionConfiguration : IEntityTypeConfiguration<CourseRevision>
{
    public void Configure(EntityTypeBuilder<CourseRevision> builder)
    {
        builder.ToTable("course_revisions");

        builder.HasKey(e => e.CourseId);
        builder.Property(e => e.Revision).IsRequired().IsConcurrencyToken();
    }
}
