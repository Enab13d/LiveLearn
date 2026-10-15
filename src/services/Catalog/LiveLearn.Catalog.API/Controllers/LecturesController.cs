using LiveLearn.BuildingBlocks;
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
        if (User.GetUserId() is not Guid tutorId)
            return this.MisconfiguredTokenProblem();

        AddLectureCommand command = new(courseId, sectionId, tutorId, request.Title, request.LectureType, request.Description);

        var result = await mediator.Send(command, ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpGet("{lectureId:guid}/content")]
    public async Task<IActionResult> GetLectureContent(Guid courseId, Guid sectionId, Guid lectureId, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid userId)
            return this.MisconfiguredTokenProblem();

        var result = await mediator.Send(
            new GetLectureContentQuery(courseId, sectionId, lectureId, userId, User.IsInRole("Admin")), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpPost("{lectureId:guid}/watch-ticket")]
    public async Task<IActionResult> IssueWatchTicket(Guid courseId, Guid lectureId, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid userId)
            return this.MisconfiguredTokenProblem();

        var result = await mediator.Send(
            new IssueWatchTicketCommand(userId, courseId, lectureId, User.IsInRole("Admin")), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpPatch("{lectureId:guid}/order")]
    public async Task<IActionResult> UpdateLectureOrder([FromBody] RequestLectureUpdateOrder request, Guid courseId, Guid sectionId, Guid lectureId, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return this.MisconfiguredTokenProblem();

        var result = await mediator.Send(
            new UpdateLectureOrderCommand(courseId, tutorId, sectionId, lectureId, request.PreviousLectureId, request.NextLectureId), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }
}
