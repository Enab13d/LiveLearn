using LiveLearn.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Catalog.Infrastructure.EntityConfigurations.Write;


internal sealed class SectionTaskConfiguration : IEntityTypeConfiguration<SectionTask>
{
    public void Configure(EntityTypeBuilder<SectionTask> builder)
    {
        builder.ToTable("section_tasks");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.TaskId).IsUnique();
        builder.Property(e => e.TaskType).HasMaxLength(16);
        builder.Property(e => e.AssignedAt)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("now()");
            
        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(e => e.CourseId);
    }
}
