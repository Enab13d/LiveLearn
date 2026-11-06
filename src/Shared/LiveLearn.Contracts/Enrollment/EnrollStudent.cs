namespace LiveLearn.Contracts.Enrollment;

// Command sent by the Ordering saga to Enrollment's queue for one paid order line.
public sealed record EnrollStudent(
    Guid OrderId,
    Guid OrderLineId,
    Guid StudentId,
    Guid CourseId
);
