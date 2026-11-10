using LiveLearn.Ordering.Domain.Enums;

namespace LiveLearn.Ordering.Domain.Common;

// A course as Ordering's projection knows it. OwnerId and Price stay null until Catalog's events arrive.
public sealed record CourseFacts(Guid CourseId, Guid? OwnerId, decimal? Price, CourseStatus Status);
