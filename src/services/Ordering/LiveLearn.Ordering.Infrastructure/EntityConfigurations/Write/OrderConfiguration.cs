using LiveLearn.Ordering.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Ordering.Infrastructure.EntityConfigurations.Write;


internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.PlacedAt).IsRequired();

        builder.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(16);

        builder.Property(e => e.Total).IsRequired().HasPrecision(19, 4);

        builder.HasIndex(e => e.StudentId);

        // xmin: concurrent saga steps and refund consumers on the same order conflict instead of overwriting
        // each other's line changes; the consumer retry re-applies on fresh state.
        builder.Property<uint>("Version").IsRowVersion();

        builder.HasMany(e => e.Lines)
            .WithOne()
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
