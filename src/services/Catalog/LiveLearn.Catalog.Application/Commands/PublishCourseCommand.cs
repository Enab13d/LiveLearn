using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record PublishCourseCommand(Guid CourseId, Guid TutorId) : ICommand, ICacheInvalidationCommand
{
    public IEnumerable<string> Tags => ["catalog"];
}
