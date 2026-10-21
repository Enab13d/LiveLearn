using LiveLearn.Assessment.Domain.Entities;
using LiveLearn.Assessment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Assessment.Infrastructure.EntityConfigurations.Write;


internal sealed class CourseTaskConfiguration : IEntityTypeConfiguration<CourseTask>
{
    public void Configure(EntityTypeBuilder<CourseTask> builder)
    {
        builder.ToTable("course_tasks");

        builder.HasKey(e => new { e.CourseId, e.TaskId });
        builder.HasOne<AssessmentTask>().WithMany().HasForeignKey(e => e.TaskId);
    }
}
