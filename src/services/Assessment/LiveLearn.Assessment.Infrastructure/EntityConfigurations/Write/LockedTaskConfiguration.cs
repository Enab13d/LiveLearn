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

        // a reused task is locked once per published course
        builder.HasKey(e => new { e.TaskId, e.CourseId });
        builder.HasOne<AssessmentTask>().WithMany().HasForeignKey(e => e.TaskId);
    }
}
