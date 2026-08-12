using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.API.Dto.Requests;
using LiveLearn.Catalog.API.Extensions;
using LiveLearn.Catalog.Application.Authorization;
using LiveLearn.Catalog.Application.Commands;
using LiveLearn.Catalog.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveLearn.Catalog.API.Controllers;

[ApiController]
[Authorize]
[Route("api/courses/{courseId:guid}/[controller]")]
public sealed class SectionsController(ISender mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetSections(Guid courseId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCourseSectionsQuery(courseId), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [AllowAnonymous]
    [HttpGet("{sectionId:guid}")]
    public async Task<IActionResult> GetSectionById(Guid courseId, Guid sectionId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetSectionByIdQuery(courseId, sectionId), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [Authorize(Policy = AuthorizationPolicies.TutorPolicy)]
    [HttpPost]
    public async Task<IActionResult> AddSection([FromBody] RequestSectionDto request, Guid courseId, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(new AddSectionCommand(courseId, tutorId, request.Title), ct);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetSectionById), new { courseId, sectionId = result.Value }, result.Value)
            : this.ToProblemResult(result.Errors);
    }

    [Authorize(Policy = AuthorizationPolicies.TutorPolicy)]
    [HttpPut("{sectionId:guid}")]
    public async Task<IActionResult> UpdateSection([FromBody] RequestSectionDto request, Guid courseId, Guid sectionId, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(new UpdateSectionCommand(courseId, tutorId, sectionId, request.Title), ct);

        return result.IsSuccess ? NoContent() : this.ToProblemResult(result.Errors);
    }

    [Authorize(Policy = AuthorizationPolicies.TutorPolicy)]
    [HttpPatch("{sectionId:guid}/order")]
    public async Task<IActionResult> UpdateSectionOrder([FromBody] RequestSectionUpdateOrder request, Guid courseId, Guid sectionId, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(new UpdateSectionOrderCommand(courseId, tutorId, sectionId, request.PreviousSectionId, request.NextSectionId), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [Authorize(Policy = AuthorizationPolicies.TutorPolicy)]
    [HttpDelete("{sectionId:guid}")]
    public async Task<IActionResult> DeleteSection(Guid courseId, Guid sectionId, CancellationToken ct)
    {
        if (User.GetUserId() is not Guid tutorId)
            return Problem("Token sub claim is not a valid identifier. Identity provider misconfigured.");

        var result = await mediator.Send(new DeleteSectionCommand(courseId, tutorId, sectionId), ct);

        return result.IsSuccess ? NoContent() : this.ToProblemResult(result.Errors);
    }
}
