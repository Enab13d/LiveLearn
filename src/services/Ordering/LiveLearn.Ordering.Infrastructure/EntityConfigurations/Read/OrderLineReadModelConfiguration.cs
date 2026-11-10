using LiveLearn.Ordering.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Ordering.Infrastructure.EntityConfigurations.Read;


internal sealed class OrderLineReadModelConfiguration : IEntityTypeConfiguration<OrderLineReadModel>
{
    public void Configure(EntityTypeBuilder<OrderLineReadModel> builder)
    {
        builder.ToTable("order_lines");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Price).HasPrecision(19, 4);
    }
}
