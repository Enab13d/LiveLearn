using LiveLearn.Catalog.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Catalog.Infrastructure.EntityConfigurations.Write;

internal sealed class TaskReplicaConfiguration : IEntityTypeConfiguration<TaskReplica>
{
    public void Configure(EntityTypeBuilder<TaskReplica> builder)
    {
        builder.ToTable("task_replicas");
        builder.HasKey(e => e.TaskId);
        builder.Property(e => e.TaskId).ValueGeneratedNever();
        builder.Property(e => e.TutorId).IsRequired();
        builder.Property(e => e.TaskType).IsRequired().HasMaxLength(16);
    }
}
