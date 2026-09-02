using LiveLearn.BuildingBlocks;
using LiveLearn.Assessment.Domain.Enums;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.Assessment.Domain.DomainEvents;

namespace LiveLearn.Assessment.Domain.Entities;


public sealed class HomeworkSubmission : AggregateRoot<Guid>
{
    private HomeworkSubmission() { }

    public Guid HomeworkId { get; private set; }

    public Guid StudentId { get; private set; }

    public Guid SectionId { get; private set; }

    public Guid CourseId { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public HomeworkStatus Status { get; private set; }

    public string? InstructorFeedback { get; private set; }

    public DateTimeOffset SubmittedAt { get; private init; } //value generated on add

    public static HomeworkSubmission Create(Guid id, Guid homeworkId, Guid studentId, Guid sectionId, Guid courseId, string content)
    {
        HomeworkSubmission homeworkSubmission = new()
        {
            Id = id,
            HomeworkId = homeworkId,
            StudentId = studentId,
            CourseId = courseId,
            SectionId = sectionId,
            Content = content,
            Status = HomeworkStatus.PendingReview
        };

        return homeworkSubmission;
    }

    public Result Review(string instructorFeedback, bool accepted)
    {
        if (Status != HomeworkStatus.PendingReview) return Result.Failure(HomeworkErrors.InvalidStatus);
        InstructorFeedback = instructorFeedback;
        if (accepted)
        {
            Status = HomeworkStatus.Accepted;
            RaiseDomainEvent(new TaskCompletedDomainEvent(HomeworkId, StudentId, SectionId, CourseId));
        }
        else
        {
            Status = HomeworkStatus.Rejected;
            RaiseDomainEvent(new TaskFailedDomainEvent(HomeworkId, StudentId, SectionId, CourseId));
        }

        return Result.Success();
    }
}
