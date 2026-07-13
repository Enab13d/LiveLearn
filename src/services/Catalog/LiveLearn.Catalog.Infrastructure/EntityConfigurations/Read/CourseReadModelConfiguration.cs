using LiveLearn.Catalog.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Catalog.Infrastructure.EntityConfigurations.Read;


internal sealed class CourseReadModelConfiguration : IEntityTypeConfiguration<CourseReadModel>
{
    public void Configure(EntityTypeBuilder<CourseReadModel> builder)
    {
        builder.ToTable("courses");
        builder.HasKey(e => e.Id);

        builder.HasMany(e => e.Sections).WithOne().HasForeignKey(e => e.CourseId);

        builder.HasOne(e => e.Category).WithMany().HasForeignKey(e => e.CategoryId);


    }

}


