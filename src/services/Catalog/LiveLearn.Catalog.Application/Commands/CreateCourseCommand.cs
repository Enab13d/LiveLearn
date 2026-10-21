using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Commands;


public sealed record CreateCourseCommand(Guid CategoryId, string Title, string Description, decimal Price) : ICommand<CourseDto>, ICacheInvalidationCommand
{
    public IEnumerable<string> Tags => ["catalog"];
}
