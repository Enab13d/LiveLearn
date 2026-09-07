using LiveLearn.Assessment.Domain.Entities;
using LiveLearn.Assessment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Assessment.Infrastructure.EntityConfigurations.Write;


internal sealed class LockedTaskConfiguration : IEntityTypeConfiguration<LockedTask>
{
    public void Configure(EntityTypeBuilder<LockedTask> builder)
    {
        builder.ToTable("locked_tasks");

        builder.HasKey(e => e.TaskId);
        builder.Property(e => e.TaskId).ValueGeneratedNever();
        builder.HasOne<AssessmentTask>().WithOne().HasForeignKey<LockedTask>(e => e.TaskId);
        builder.Property(e => e.CourseId).IsRequired();
    }
}
