using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Domain.Enums;

namespace LiveLearn.Enrollment.Domain.Entities;


public sealed class EnrollmentItem : Entity<Guid>
{
    public Guid EnrollmentId { get; private set; }

    public Guid ItemId { get; private set; }

    public EnrollmentItemKind Kind { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public bool IsCompleted => CompletedAt is not null;

    private EnrollmentItem() { }

    internal static EnrollmentItem Create(Guid enrollmentId, Guid itemId, EnrollmentItemKind kind, DateTimeOffset? completedAt = null)
    {
        return new EnrollmentItem()
        {
            Id = Guid.NewGuid(),
            EnrollmentId = enrollmentId,
            ItemId = itemId,
            Kind = kind,
            CompletedAt = completedAt,
        };
    }

    internal void Complete(DateTimeOffset completedAt)
    {
        CompletedAt = completedAt;
    }

}
