namespace LiveLearn.Assessment.Infrastructure.Models;


internal sealed class HomeworkSubmissionReadModel
{
    public Guid Id { get; set; }
    public Guid HomeworkId { get; set; }

    public Guid StudentId { get; set; }

    public Guid SectionId { get; set; }

    public Guid CourseId { get; set; }

    public string Content { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? InstructorFeedback { get; set; }

    public DateTimeOffset SubmittedAt { get; set; }
}
