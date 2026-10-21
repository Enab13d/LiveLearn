using LiveLearn.Catalog.API.Dto.Requests;
using LiveLearn.Catalog.API.Extensions;
using LiveLearn.Catalog.Application.Commands;
using LiveLearn.Catalog.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveLearn.Catalog.API.Controllers;

[ApiController]
[Authorize]
[Route("api/courses/{courseId:guid}/lectures/{lectureId:guid}/video")]
public sealed class LectureVideoController(ISender mediator) : ControllerBase
{
    [HttpPost("upload-url")]
    public async Task<IActionResult> RequestVideoUpload(Guid courseId, Guid lectureId, CancellationToken ct)
    {
        var result = await mediator.Send(new RequestLectureVideoUploadCommand(courseId, lectureId), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpPatch("uploaded")]
    public async Task<IActionResult> MarkVideoUploaded([FromBody] RequestMarkVideoUploadedDto request, Guid courseId, Guid lectureId, CancellationToken ct)
    {
        var result = await mediator.Send(new MarkLectureVideoUploadedCommand(courseId, lectureId, request.VideoId), ct);

        return result.IsSuccess ? NoContent() : this.ToProblemResult(result.Errors);
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetVideoStatus(Guid courseId, Guid lectureId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetLectureVideoStatusQuery(courseId, lectureId), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }
}
