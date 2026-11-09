using LiveLearn.Ordering.Domain.Enums;

namespace LiveLearn.Ordering.Infrastructure.Models;

// Local copy of a Catalog course (O9). Catalog events arrive in any order, so every apply is order-tolerant:
// status only moves forward (Archived is terminal) and a price is taken only if newer than PriceUpdatedAt.
// Ownership never changes in Catalog, so OwnerId is simply set.
public sealed class CourseProjection
{
    public Guid CourseId { get; private set; }

    // Null until CoursePublishedEvent arrives.
    public Guid? OwnerId { get; private set; }

    public decimal? Price { get; private set; }

    // When Catalog last set Price — PublishedAt / OccurredOn of the event that carried it. Lets an older
    // price delivered late be ignored. Null until a price arrives.
    public DateTimeOffset? PriceUpdatedAt { get; private set; }

    public CourseStatus Status { get; private set; }

    private CourseProjection() { }

    public static CourseProjection Create(Guid courseId) => new() { CourseId = courseId, Status = CourseStatus.Unknown };

    public void ApplyPublished(Guid ownerId, decimal price, DateTimeOffset publishedAt)
    {
        OwnerId = ownerId;
        if (Status != CourseStatus.Archived) Status = CourseStatus.Published;
        ApplyPrice(price, publishedAt);
    }

    public void ApplyPrice(decimal price, DateTimeOffset updatedAt)
    {
        if (PriceUpdatedAt.HasValue && updatedAt <= PriceUpdatedAt.Value) return;

        Price = price;
        PriceUpdatedAt = updatedAt;
    }

    public void Archive() => Status = CourseStatus.Archived;
}
