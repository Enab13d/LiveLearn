using LiveLearn.Catalog.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Catalog.Infrastructure.EntityConfigurations.Read;

internal sealed class CategoryReadModelConfiguration : IEntityTypeConfiguration<CategoryReadModel>
{
    public void Configure(EntityTypeBuilder<CategoryReadModel> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Slug).IsUnique();

        builder.HasMany<CourseReadModel>().WithOne().HasForeignKey(e => e.CategoryId);
    }
}



