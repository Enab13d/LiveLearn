using LiveLearn.BuildingBlocks;

namespace LiveLearn.Ordering.Application.Commands;


// Adds a course to the current student's cart after the O4 checks (O3).
public sealed record AddToCartCommand(Guid CourseId) : ICommand;
