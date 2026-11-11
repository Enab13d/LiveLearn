using LiveLearn.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Payment.Infrastructure.EntityConfigurations.Write;


internal sealed class OrderPaymentLineConfiguration : IEntityTypeConfiguration<OrderPaymentLine>
{
    public void Configure(EntityTypeBuilder<OrderPaymentLine> builder)
    {
        builder.ToTable("order_payment_lines");

        // Ids are set by the domain; without this, lines of a tracked payment would be treated as existing rows.
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.CourseTitle).IsRequired().HasMaxLength(128);

        builder.ComplexProperty(e => e.Price, money =>
        {
            money.Property(m => m.Amount).IsRequired().HasPrecision(19, 4);
            money.Property(m => m.Currency).IsRequired().HasMaxLength(3);
        });

        builder.Property(e => e.TutorShareRate).IsRequired().HasPrecision(5, 4);

        builder.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(32);

        builder.Property(e => e.IsEnrolled).IsRequired();

        builder.Property(e => e.RefundId).HasMaxLength(255);
    }
}
