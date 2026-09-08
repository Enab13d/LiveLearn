using LiveLearn.Assessment.API.Dto.QueryParameters;
using LiveLearn.Assessment.API.Dto.Requests;
using LiveLearn.Assessment.API.Extensions;
using LiveLearn.Assessment.Application.Authorization;
using LiveLearn.Assessment.Application.Commands;
using LiveLearn.Assessment.Application.Queries;
using LiveLearn.BuildingBlocks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveLearn.Assessment.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class HomeworkController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.TutorPolicy)]
    public async Task<IActionResult> CreateHomework([FromBody] CreateHomeworkRequestDto request, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(
            new CreateHomeworkCommand(tutorId, request.Title, request.Description), ct);

        return result.IsSuccess ? CreatedAtAction(nameof(GetHomework), new { taskId = result.Value }, result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpGet]
    [Route("{taskId:guid}")]
    public async Task<IActionResult> GetHomework(Guid taskId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetHomeworkByIdQuery(taskId), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpPatch]
    [Route("{taskId:guid}/title")]
    [Authorize(Policy = AuthorizationPolicies.TutorPolicy)]
    public async Task<IActionResult> UpdateTitle(Guid taskId, [FromBody] UpdateHomeworkTitleRequestDto request, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(new UpdateHomeworkTitleCommand(taskId, tutorId, request.Title), ct);

        return result.IsSuccess ? Ok() : this.ToProblemResult(result.Errors);
    }

    [HttpPatch]
    [Route("{taskId:guid}/description")]
    [Authorize(Policy = AuthorizationPolicies.TutorPolicy)]
    public async Task<IActionResult> UpdateDescription(Guid taskId, [FromBody] UpdateHomeworkDescriptionRequestDto request, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(new UpdateHomeworkDescriptionCommand(taskId, tutorId, request.Description), ct);

        return result.IsSuccess ? Ok() : this.ToProblemResult(result.Errors);
    }

    [HttpPost]
    [Route("{taskId:guid}/submissions")]
    [Authorize(Policy = AuthorizationPolicies.StudentPolicy)]
    public async Task<IActionResult> SubmitHomework(Guid taskId, [FromBody] SubmitHomeworkRequestDto request, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid studentId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(new SubmitHomeworkCommand(studentId, taskId, request.Content), ct);

        return result.IsSuccess ? Ok() : this.ToProblemResult(result.Errors);
    }

    [HttpGet]
    [Route("{taskId:guid}/submissions")]
    [Authorize(Policy = AuthorizationPolicies.TutorPolicy)]
    public async Task<IActionResult> GetSubmissions(
        Guid taskId,
        [FromQuery] HomeworkQueryParameters query,
        CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(
            new GetHomeworkSubmissionsQuery(
                taskId,
                tutorId,
                query.PageNumber,
                query.PageSize,
                query.StudentId,
                query.CourseId,
                query.SectionId,
                query.Status,
                query.StartDate,
                query.EndDate),
            ct);

        return result.IsSuccess ? Ok() : this.ToProblemResult(result.Errors);
    }

    [HttpPatch]
    [Route("submissions/{submissionId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.TutorPolicy)]
    public async Task<IActionResult> ReviewHomework(Guid submissionId, ReviewHomeworkSubmissionRequestDto request, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(
            new ReviewHomeworkSubmissionCommand(tutorId, submissionId, request.Feedback, request.IsAccepted), ct);

        return result.IsSuccess ? Ok() : this.ToProblemResult(result.Errors);
    }


}
