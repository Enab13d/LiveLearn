using LiveLearn.Enrollment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Enrollment.Infrastructure.EntityConfigurations.Read;

internal sealed class EnrollmentItemReadModelConfiguration : IEntityTypeConfiguration<EnrollmentItemReadModel>
{
    public void Configure(EntityTypeBuilder<EnrollmentItemReadModel> builder)
    {
        builder.ToTable("enrollment_items");
        builder.HasKey(e => e.Id);
    }
}
