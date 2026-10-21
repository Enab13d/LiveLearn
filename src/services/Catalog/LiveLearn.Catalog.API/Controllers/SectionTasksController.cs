using LiveLearn.Catalog.API.Dto.Requests;
using LiveLearn.Catalog.API.Extensions;
using LiveLearn.Catalog.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveLearn.Catalog.API.Controllers;

[ApiController]
[Authorize]
[Route("api/courses/{courseId:guid}/sections/{sectionId:guid}/[controller]")]
public sealed class SectionTasksController(ISender mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AssignTaskToSection([FromBody] RequestSectionTaskDto request, Guid courseId, Guid sectionId, CancellationToken ct)
    {
        var result = await mediator.Send(new AssignTaskToSectionCommand(courseId, sectionId, request.TaskId), ct);

        return result.IsSuccess ? Ok() : this.ToProblemResult(result.Errors);
    }

    [HttpDelete]
    [Route("{taskId:guid}")]
    public async Task<IActionResult> UnassignTaskFromSection(Guid courseId, Guid sectionId, Guid taskId, CancellationToken ct)
    {
        var result = await mediator.Send(new UnassignTaskFromSectionCommand(courseId, sectionId, taskId), ct);

        return result.IsSuccess ? NoContent() : this.ToProblemResult(result.Errors);
    }
}
