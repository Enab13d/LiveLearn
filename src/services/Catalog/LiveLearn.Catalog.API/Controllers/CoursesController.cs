using LiveLearn.Catalog.API.Dto.Requests;
using LiveLearn.Catalog.API.Extensions;
using LiveLearn.Catalog.Application.Commands;
using LiveLearn.Catalog.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveLearn.Catalog.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class CoursesController(ISender mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetCatalog(
        CancellationToken ct,
        int pageNumber = 1,
        int pageSize = 20,
        Guid? categoryId = null,
        Guid? tutorId = null,
        decimal? maxPrice = null,
        string? query = null
    )
    {
        var result = await mediator.Send(new GetCatalogQuery(pageNumber, pageSize, categoryId, tutorId, maxPrice, query), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [AllowAnonymous]
    [HttpGet("{courseId:guid}")]
    public async Task<IActionResult> GetCourseDetails(Guid courseId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCourseByIdQuery(courseId), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpGet("{courseId:guid}/outline")]
    public async Task<IActionResult> GetLearningOutline(Guid courseId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetLearningOutlineQuery(courseId), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }


    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] RequestCourseDto request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CreateCourseCommand(request.CategoryId, request.Title, request.Description, request.Price), ct);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCourseDetails), new { courseId = result.Value.Id }, result.Value)
            : this.ToProblemResult(result.Errors);
    }

    [HttpPut("{courseId:guid}")]
    public async Task<IActionResult> UpdateCourse(Guid courseId, [FromBody] RequestCourseDto request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateCourseCommand(courseId, request.Title, request.Description, request.Price, request.CategoryId), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpPost("{courseId:guid}/publish")]
    public async Task<IActionResult> PublishCourse(Guid courseId, CancellationToken ct)
    {
        var result = await mediator.Send(new PublishCourseCommand(courseId), ct);

        return result.IsSuccess ? Ok() : this.ToProblemResult(result.Errors);
    }

}
