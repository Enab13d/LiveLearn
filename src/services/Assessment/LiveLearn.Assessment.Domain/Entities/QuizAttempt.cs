using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Domain.Entities;


public sealed class QuizAttempt : AggregateRoot<Guid>
{
    private QuizAttempt() { }

    public Guid QuizId { get; private init; }

    public Guid StudentId { get; private init; }

    public Guid SectionId { get; private init; }

    public Guid CourseId { get; private init; }

    public int Score { get; private init; }

    public bool IsPassed { get; private init; }

    public DateTimeOffset AttemptedAt { get; private init; }

    public static QuizAttempt Create(Guid id, Guid quizId, Guid studentId, Guid sectionId, Guid courseId, int score, bool isPassed)
    {
        QuizAttempt quizAttempt = new()
        {
            Id = id,
            QuizId = quizId,
            StudentId = studentId,
            SectionId = sectionId,
            CourseId = courseId,
            Score = score,
            IsPassed = isPassed
        };

        return quizAttempt;
    }
}
