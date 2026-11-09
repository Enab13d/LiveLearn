namespace LiveLearn.Ordering.Domain.Enums;

// Catalog course status as seen by Ordering's projection. Moves only forward: Unknown → Published → Archived.
public enum CourseStatus
{
    // Only a price change or nothing has arrived yet — CoursePublishedEvent is still on its way.
    Unknown,
    Published,
    // Catalog's unpublish; terminal.
    Archived
}
