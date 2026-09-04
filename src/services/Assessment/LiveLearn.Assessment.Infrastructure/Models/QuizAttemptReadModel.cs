namespace LiveLearn.Assessment.Infrastructure.Models;

internal sealed class QuizAttemptReadModel
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }

    public Guid StudentId { get; set; }

    public Guid SectionId { get; set; }

    public Guid CourseId { get; set; }

    public int Score { get; set; }

    public bool IsPassed { get; set; }

    public DateTimeOffset AttemptedAt { get; set; }
}
