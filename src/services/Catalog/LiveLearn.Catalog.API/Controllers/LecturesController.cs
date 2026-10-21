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
[Route("api/courses/{courseId:guid}/sections/{sectionId:guid}/[controller]")]
public sealed class LecturesController(ISender mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddLecture([FromBody] RequestLectureDto request, Guid courseId, Guid sectionId, CancellationToken ct)
    {
        AddLectureCommand command = new(courseId, sectionId, request.Title, request.LectureType, request.Description);

        var result = await mediator.Send(command, ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpGet("{lectureId:guid}/content")]
    public async Task<IActionResult> GetLectureContent(Guid courseId, Guid sectionId, Guid lectureId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetLectureContentQuery(courseId, sectionId, lectureId), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpPost("{lectureId:guid}/watch-ticket")]
    public async Task<IActionResult> IssueWatchTicket(Guid courseId, Guid sectionId, Guid lectureId, CancellationToken ct)
    {
        var result = await mediator.Send(new IssueWatchTicketCommand(courseId, sectionId, lectureId), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpPatch("{lectureId:guid}/order")]
    public async Task<IActionResult> UpdateLectureOrder([FromBody] RequestLectureUpdateOrder request, Guid courseId, Guid sectionId, Guid lectureId, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateLectureOrderCommand(courseId, sectionId, lectureId, request.PreviousLectureId, request.NextLectureId), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpPost("{lectureId:guid}/archive")]
    public async Task<IActionResult> ArchiveLecture(Guid courseId, Guid sectionId, Guid lectureId, CancellationToken ct)
    {
        var result = await mediator.Send(new ArchiveLectureCommand(courseId, sectionId, lectureId), ct);

        return result.IsSuccess ? NoContent() : this.ToProblemResult(result.Errors);
    }

    [HttpPatch("{lectureId:guid}/section")]
    public async Task<IActionResult> MoveLecture([FromBody] RequestMoveLectureDto request, Guid courseId, Guid sectionId, Guid lectureId, CancellationToken ct)
    {
        var command = new MoveLectureCommand(courseId, sectionId, lectureId, request.TargetSectionId);
        var result = await mediator.Send(command, ct);
        return result.IsSuccess ? NoContent() : this.ToProblemResult(result.Errors);
    }
}
