using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record PublishCourseCommand(Guid CourseId) : ICommand, ICacheInvalidationCommand
{
    public IEnumerable<string> Tags => ["catalog"];
}
