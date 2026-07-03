using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Commands;

internal sealed class CreateCourseCommandHandler() : ICommandHandler<CreateCourseCommand, CourseDto>
{
    public Task<Result<CourseDto>> Handle(CreateCourseCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
