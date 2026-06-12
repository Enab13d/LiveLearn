using LiveLearn.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Identity.Infrastructure.EntityConfigurations;

internal sealed class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(32);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(32);
        builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(64);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(254);
        builder.Property(u => u.AvatarUrl).IsRequired(false).HasMaxLength(2048);
        builder.Property(u => u.Bio).IsRequired(false).HasMaxLength(1000);
        builder.Property(u => u.Role).HasConversion<string>();

        builder.HasIndex(u => u.Email).IsUnique();
    }
}
