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
public sealed class QuizzesController(ISender mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizRequestDto request, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateQuizCommand(request.Title, request.PassingScore), ct);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetQuiz), new { taskId = result.Value }, result.Value)
            : this.ToProblemResult(result.Errors);
    }


    [HttpPost]
    [Route("{taskId:guid}/questions")]
    public async Task<IActionResult> AddQuestion(Guid taskId, [FromBody] AddQuestionToQuizRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new AddQuestionToQuizCommand(taskId, request.Text, request.Answers, request.CorrectAnswerIdx), ct);

        return result.IsSuccess
            ? Created()
            : this.ToProblemResult(result.Errors);
    }

    [HttpDelete]
    [Route("{taskId:guid}/questions/{questionId:guid}")]
    public async Task<IActionResult> RemoveQuestion(Guid taskId, Guid questionId, CancellationToken ct)
    {
        var result = await mediator.Send(new RemoveQuestionFromQuizCommand(taskId, questionId), ct);

        return result.IsSuccess
            ? NoContent()
            : this.ToProblemResult(result.Errors);
    }

    [HttpGet]
    [Route("{taskId:guid}")]
    public async Task<IActionResult> GetQuiz(Guid taskId, Guid? courseId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetQuizByIdQuery(taskId, courseId), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : this.ToProblemResult(result.Errors);
    }

    [HttpPost]
    [Route("{taskId:guid}/attempts")]
    public async Task<IActionResult> SubmitAttempt(Guid taskId, [FromBody] SubmitQuizAttemptRequestDto request, CancellationToken ct)
    {
        var result = await mediator.Send(new SubmitQuizAttemptCommand(request.CourseId, taskId, request.Answers), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : this.ToProblemResult(result.Errors);

    }

    [HttpGet]
    [Route("{taskId:guid}/attempts")]
    public async Task<IActionResult> GetAttempts(Guid taskId, [FromQuery] PageableQueryParams query, CancellationToken ct)
    {
        var result = await mediator.Send(new GetQuizAttemptsQuery(taskId, query.PageNumber, query.PageSize), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : this.ToProblemResult(result.Errors);
    }

    [HttpPatch]
    [Route("{taskId:guid}")]
    public async Task<IActionResult> UpdateTitle(Guid taskId, [FromBody] UpdateQuizTitleRequestDto request, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateQuizTitleCommand(taskId, request.Title), ct);

        return result.IsSuccess
            ? Ok()
            : this.ToProblemResult(result.Errors);
    }


}
