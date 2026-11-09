namespace LiveLearn.Ordering.Domain.Common;

// A course as taken from the projection at placement: owner and price are frozen into the order line.
public sealed record OrderLineSnapshot(Guid CourseId, Guid TutorId, decimal Price);
