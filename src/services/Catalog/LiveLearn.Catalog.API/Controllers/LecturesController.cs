using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.API.Dto.Requests;
using LiveLearn.Catalog.API.Extensions;
using LiveLearn.Catalog.Application.Authorization;
using LiveLearn.Catalog.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveLearn.Catalog.API.Controllers;

[ApiController]
[Authorize]
[Route("api/courses/{courseId:guid}/sections/{sectionId:guid}/[controller]")]
public sealed class LecturesController(ISender mediator) : ControllerBase
{

    [Authorize(Policy = AuthorizationPolicies.TutorPolicy)]
    [HttpPost]
    public async Task<IActionResult> AddLecture([FromBody] RequestLectureDto request, Guid courseId, Guid sectionId, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        AddLectureCommand command = new(courseId, sectionId, tutorId, request.Title, request.LectureType, request.Description);

        var result = await mediator.Send(command, ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [Authorize(Policy = AuthorizationPolicies.TutorPolicy)]
    [HttpPatch("{lectureId:guid}/order")]
    public async Task<IActionResult> UpdateLectureOrder([FromBody] RequestLectureUpdateOrder request, Guid courseId, Guid sectionId, Guid lectureId, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(
            new UpdateLectureOrderCommand(courseId, tutorId, sectionId, lectureId, request.PreviousLectureId, request.NextLectureId), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }
}
