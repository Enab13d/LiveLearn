using LiveLearn.Enrollment.API.Dto.QueryParameters;
using LiveLearn.Enrollment.API.Extensions;
using LiveLearn.Enrollment.Application.Authorization;
using LiveLearn.Enrollment.Application.Commands;
using LiveLearn.Enrollment.Application.Queries;
using LiveLearn.Enrollment.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveLearn.Enrollment.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class EnrollmentsController(ISender mediator) : ControllerBase
{
    [HttpGet("my")]
    public async Task<IActionResult> GetMyEnrollments([FromQuery] PageableQueryParams query, CancellationToken ct)
    {
        var result = await mediator.Send(new GetStudentEnrollmentsQuery(query.PageNumber, query.PageSize), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEnrollment(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetEnrollmentQuery(id), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpGet("my/{courseId:guid}/status")]
    public async Task<IActionResult> GetMyEnrollmentStatus(Guid courseId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetMyEnrollmentStatusQuery(courseId), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }

    [HttpPost("my/{courseId:guid}/lectures/{lectureId:guid}/complete")]
    public async Task<IActionResult> CompleteLecture(Guid courseId, Guid lectureId, CancellationToken ct)
    {
        var result = await mediator.Send(new CompleteLectureCommand(courseId, lectureId), ct);

        return result.IsSuccess ? NoContent() : this.ToProblemResult(result.Errors);
    }

    [HttpDelete("{studentId:guid}/{courseId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.AdminPolicy)]
    public async Task<IActionResult> RemoveEnrollment(Guid studentId, Guid courseId, CancellationToken ct)
    {
        var result = await mediator.Send(new RevokeEnrollmentCommand(studentId, courseId, RevocationReason.AdminRemoval), ct);

        return result.IsSuccess ? NoContent() : this.ToProblemResult(result.Errors);
    }

    [HttpPost("snapshots/republish")]
    [Authorize(Policy = AuthorizationPolicies.AdminPolicy)]
    public async Task<IActionResult> RepublishSnapshots(CancellationToken ct)
    {
        var result = await mediator.Send(new RepublishEnrollmentSnapshotsCommand(), ct);

        return result.IsSuccess ? Ok(result.Value) : this.ToProblemResult(result.Errors);
    }
}
