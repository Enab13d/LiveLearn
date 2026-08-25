
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record ArchiveCourseCommand(Guid CourseId, Guid TutorId) : ICommand;
