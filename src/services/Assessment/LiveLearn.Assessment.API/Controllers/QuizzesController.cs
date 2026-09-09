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
public sealed class QuizzesController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.TutorPolicy)]
    public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizRequestDto request, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(new CreateQuizCommand(tutorId, request.Title, request.PassingScore), ct);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetQuiz), new { taskId = result.Value }, result.Value)
            : this.ToProblemResult(result.Errors);
    }


    [HttpPost]
    [Route("{taskId:guid}/questions")]
    [Authorize(Policy = AuthorizationPolicies.TutorPolicy)]
    public async Task<IActionResult> AddQuestion(Guid taskId, [FromBody] AddQuestionToQuizRequest request, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(
            new AddQuestionToQuizCommand(tutorId, taskId, request.Text, request.Answers, request.CorrectAnswerIdx), ct);

        return result.IsSuccess
            ? Created()
            : this.ToProblemResult(result.Errors);
    }

    [HttpDelete]
    [Route("{taskId:guid}/questions/{questionId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.TutorPolicy)]
    public async Task<IActionResult> RemoveQuestion(Guid taskId, Guid questionId, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(new RemoveQuestionFromQuizCommand(taskId, tutorId, questionId), ct);

        return result.IsSuccess
            ? NoContent()
            : this.ToProblemResult(result.Errors);
    }

    [HttpGet]
    [Route("{taskId:guid}")]
    public async Task<IActionResult> GetQuiz(Guid taskId, CancellationToken ct)
    {
        var isTutor = User.IsInRole(Role.Tutor.ToString());

        var result = await mediator.Send(new GetQuizByIdQuery(taskId, IncludeCorrectAnswer: isTutor), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : this.ToProblemResult(result.Errors);
    }

    [HttpPost]
    [Route("{taskId:guid}/attempts")]
    [Authorize(Policy = AuthorizationPolicies.StudentPolicy)]
    public async Task<IActionResult> SubmitAttempt(Guid taskId, [FromBody] SubmitQuizAttemptRequestDto request, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid studentId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(new SubmitQuizAttemptCommand(studentId, taskId, request.Answers), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : this.ToProblemResult(result.Errors);

    }

    [HttpGet]
    [Route("{taskId:guid}/attempts")]
    [Authorize(Policy = AuthorizationPolicies.StudentPolicy)]
    public async Task<IActionResult> GetAttempts(Guid taskId, [FromQuery] PageableQueryParams query, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid studentId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(new GetQuizAttemptsQuery(studentId, taskId, query.PageNumber, query.PageSize), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : this.ToProblemResult(result.Errors);
    }

    [HttpPatch]
    [Route("{taskId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.TutorPolicy)]
    public async Task<IActionResult> UpdateTitle(Guid taskId, [FromBody] UpdateQuizTitleRequestDto request, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(new UpdateQuizTitleCommand(tutorId, taskId, request.Title), ct);
        
        return result.IsSuccess
            ? Ok()
            : this.ToProblemResult(result.Errors);
    }


}
