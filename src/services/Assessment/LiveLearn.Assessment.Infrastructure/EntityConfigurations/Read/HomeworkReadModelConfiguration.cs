using LiveLearn.Assessment.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Assessment.Infrastructure.EntityConfigurations.Read;


internal sealed class HomeworkReadModelConfiguration : IEntityTypeConfiguration<HomeworkReadModel>
{
    public void Configure(EntityTypeBuilder<HomeworkReadModel> builder)
    {
        builder.ToTable("tasks");
        builder.Property<string>("TaskType");
        builder.HasQueryFilter(e => EF.Property<string>(e, "TaskType") == "Homework");
    }
}
