using LiveLearn.Payment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Payment.Infrastructure.EntityConfigurations.Read;


internal sealed class OrderPaymentReadModelConfiguration : IEntityTypeConfiguration<OrderPaymentReadModel>
{
    public void Configure(EntityTypeBuilder<OrderPaymentReadModel> builder)
    {
        builder.ToTable("order_payments");
        builder.HasKey(e => e.Id);
    }
}
