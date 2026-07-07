using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record PublishCourseCommand(Guid CourseId, Guid TutorId) : ICommand;
