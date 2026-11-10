using LiveLearn.Ordering.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Ordering.Infrastructure.EntityConfigurations.Write;


internal sealed class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.ToTable("order_lines");

        // Ids are set by the domain; without this, lines of a tracked order would be treated as existing rows.
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Price).IsRequired().HasPrecision(19, 4);

        builder.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(32);

        builder.Ignore(e => e.IsFinal);

        // Backstop only: the placement handler guards duplicate active lines; this catches the race (O5, O6).
        builder.HasIndex(e => new { e.StudentId, e.CourseId })
            .IsUnique()
            .HasFilter("\"Status\" IN ('Pending', 'Enrolled')");
    }
}
