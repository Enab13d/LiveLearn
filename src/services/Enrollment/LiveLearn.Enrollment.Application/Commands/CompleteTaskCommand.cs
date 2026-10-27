using LiveLearn.BuildingBlocks;

namespace LiveLearn.Enrollment.Application.Commands;

// Sent by TaskCompletedConsumer, so the student id comes from the event.
public sealed record CompleteTaskCommand(Guid StudentId, Guid CourseId, Guid TaskId) : ICommand;
