using LiveLearn.Enrollment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Enrollment.Infrastructure.EntityConfigurations.Write;


internal sealed class EnrollmentItemConfiguration : IEntityTypeConfiguration<EnrollmentItem>
{
    public void Configure(EntityTypeBuilder<EnrollmentItem> builder)
    {
        builder.ToTable("enrollment_items");

        // Ids are set by the domain; without this, items added to a tracked enrollment would be treated as existing rows.
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Kind).IsRequired().HasConversion<string>().HasMaxLength(16);

        builder.Property(e => e.CompletedAt).IsRequired(false);

        builder.Ignore(e => e.IsCompleted);

        builder.HasIndex(e => new { e.EnrollmentId, e.ItemId }).IsUnique();
    }
}
