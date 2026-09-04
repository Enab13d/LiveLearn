using LiveLearn.Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveLearn.Assessment.Infrastructure.EntityConfigurations.Write;


internal sealed class HomeworkSubmissionConfiguration : IEntityTypeConfiguration<HomeworkSubmission>
{
    public void Configure(EntityTypeBuilder<HomeworkSubmission> builder)
    {
        builder.ToTable("homework_submissions");

        builder.HasKey(e => e.Id);
        builder.HasOne<Homework>()
            .WithMany()
            .HasForeignKey(e => e.HomeworkId);

        builder.Property(e => e.CourseId).IsRequired();
        builder.Property(e => e.SectionId).IsRequired();
        builder.Property(e => e.StudentId).IsRequired();
        builder.Property(e => e.Content).IsRequired();
        builder.Property(e => e.Status).IsRequired().HasConversion<string>();
        builder.Property(e => e.InstructorFeedback).HasMaxLength(2048);
        builder.Property(e => e.SubmittedAt).ValueGeneratedOnAdd().HasDefaultValueSql("now()");

        builder.HasIndex(e => new {e.HomeworkId, e.StudentId, e.SubmittedAt})
            .IsDescending()
            .HasDatabaseName("IX_HomeworkSubmissions_Homework_Student_SubmittedAt");
    }
}
