using LiveLearn.BuildingBlocks;

namespace LiveLearn.Enrollment.Application.Commands;

// Sent by EnrollStudentConsumer for each paid order line; the consumer reports the result to the saga.
public sealed record EnrollStudentCommand(
    Guid OrderId,
    Guid OrderLineId,
    Guid StudentId,
    Guid CourseId) : ICommand;
