using LiveLearn.BuildingBlocks;

namespace LiveLearn.Enrollment.Application.Commands;

// Sent by PaymentConfirmedConsumer, so the student id comes from the event.
public sealed record CreateEnrollmentCommand(Guid StudentId, Guid CourseId) : ICommand;
