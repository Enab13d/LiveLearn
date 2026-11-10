using LiveLearn.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Payment.Infrastructure.EntityConfigurations.Write;


internal sealed class OrderPaymentConfiguration : IEntityTypeConfiguration<OrderPayment>
{
    public void Configure(EntityTypeBuilder<OrderPayment> builder)
    {
        builder.ToTable("order_payments");

        builder.Property(e => e.Id).ValueGeneratedNever();

        // One payment per order: a redelivered RequestPayment that races past the handler's guard fails here (D10, D13).
        builder.HasIndex(e => e.OrderId).IsUnique();

        builder.HasIndex(e => e.StudentId);

        builder.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(32);

        builder.Property(e => e.CreatedAt).IsRequired();

        builder.ComplexProperty(e => e.Total, money =>
        {
            money.Property(m => m.Amount).IsRequired().HasPrecision(19, 4);
            money.Property(m => m.Currency).IsRequired().HasMaxLength(3);
        });

        builder.ComplexProperty(e => e.Fee, money =>
        {
            money.Property(m => m.Amount).IsRequired().HasPrecision(19, 4);
            money.Property(m => m.Currency).IsRequired().HasMaxLength(3);
        });

        builder.Property(e => e.CheckoutSessionId).HasMaxLength(255);

        builder.Property(e => e.CheckoutUrl).HasMaxLength(2048);

        builder.Property(e => e.ChargeId).HasMaxLength(255);

        // xmin: a webhook and a consumer changing the same payment conflict instead of overwriting each
        // other; DbUpdateConcurrencyException propagates and the entry point retries on fresh state (D9).
        builder.Property<uint>("Version").IsRowVersion();

        builder.HasMany(e => e.Lines)
            .WithOne()
            .HasForeignKey(e => e.OrderPaymentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
