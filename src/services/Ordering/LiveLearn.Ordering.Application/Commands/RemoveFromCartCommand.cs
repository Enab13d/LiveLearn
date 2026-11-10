using LiveLearn.BuildingBlocks;

namespace LiveLearn.Ordering.Application.Commands;


// Removes a course from the current student's cart; a course that isn't there is a no-op (O11).
public sealed record RemoveFromCartCommand(Guid CourseId) : ICommand;
