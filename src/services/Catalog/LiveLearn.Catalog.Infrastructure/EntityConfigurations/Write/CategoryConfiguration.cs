

using LiveLearn.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Catalog.Infrastructure.EntityConfigurations.Write;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.Property(e => e.Name).IsRequired().HasMaxLength(128);
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.Property(e => e.Slug).IsRequired().HasMaxLength(32);

        builder.HasMany<Course>().WithOne().HasForeignKey(e => e.CategoryId);
    }
}
