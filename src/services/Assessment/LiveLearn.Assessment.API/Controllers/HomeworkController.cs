using LiveLearn.Assessment.API.Dto.QueryParameters;
using LiveLearn.Assessment.API.Dto.Requests;
using LiveLearn.Assessment.API.Extensions;
using LiveLearn.Assessment.Application.Authorization;
using LiveLearn.Assessment.Application.Commands;
using LiveLearn.Assessment.Application.Queries;
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
    public async Task<IActionResult> CreateHomework([FromBody] CreateHomeworkRequestDto request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CreateHomeworkCommand(request.Title, request.Description), ct);

        return result.IsSuccess ? CreatedAtAction(nameof(GetHomework), new { taskId = result.Value }, result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpGet]
    [Route("{taskId:guid}")]
    public async Task<IActionResult> GetHomework(Guid taskId, Guid? courseId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetHomeworkByIdQuery(taskId, courseId), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpPatch]
    [Route("{taskId:guid}/title")]
    public async Task<IActionResult> UpdateTitle(Guid taskId, [FromBody] UpdateHomeworkTitleRequestDto request, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateHomeworkTitleCommand(taskId, request.Title), ct);

        return result.IsSuccess ? Ok() : this.ToProblemResult(result.Errors);
    }

    [HttpPatch]
    [Route("{taskId:guid}/description")]
    public async Task<IActionResult> UpdateDescription(Guid taskId, [FromBody] UpdateHomeworkDescriptionRequestDto request, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateHomeworkDescriptionCommand(taskId, request.Description), ct);

        return result.IsSuccess ? Ok() : this.ToProblemResult(result.Errors);
    }

    [HttpPost]
    [Route("{taskId:guid}/submissions")]
    public async Task<IActionResult> SubmitHomework(Guid taskId, [FromBody] SubmitHomeworkRequestDto request, CancellationToken ct)
    {
        var result = await mediator.Send(new SubmitHomeworkCommand(request.CourseId, taskId, request.Content), ct);

        return result.IsSuccess ? Ok() : this.ToProblemResult(result.Errors);
    }

    [HttpGet]
    [Route("{taskId:guid}/submissions")]
    public async Task<IActionResult> GetSubmissions(
        Guid taskId,
        [FromQuery] HomeworkQueryParameters query,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetHomeworkSubmissionsQuery(
                taskId,
                query.PageNumber,
                query.PageSize,
                query.StudentId,
                query.CourseId,
                query.Status,
                query.StartDate,
                query.EndDate),
            ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpPatch]
    [Route("submissions/{submissionId:guid}")]
    public async Task<IActionResult> ReviewHomework(Guid submissionId, ReviewHomeworkSubmissionRequestDto request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new ReviewHomeworkSubmissionCommand(submissionId, request.Feedback, request.IsAccepted), ct);

        return result.IsSuccess ? Ok() : this.ToProblemResult(result.Errors);
    }


}
