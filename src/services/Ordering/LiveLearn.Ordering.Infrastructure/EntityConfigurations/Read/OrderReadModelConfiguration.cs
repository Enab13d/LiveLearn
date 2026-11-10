using LiveLearn.Ordering.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Ordering.Infrastructure.EntityConfigurations.Read;


internal sealed class OrderReadModelConfiguration : IEntityTypeConfiguration<OrderReadModel>
{
    public void Configure(EntityTypeBuilder<OrderReadModel> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Total).HasPrecision(19, 4);
        builder.HasMany(e => e.Lines).WithOne().HasForeignKey(e => e.OrderId);
    }
}
