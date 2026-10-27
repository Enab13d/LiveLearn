using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Application.Dto;
using LiveLearn.Enrollment.Application.Queries;
using LiveLearn.Enrollment.Application.Services;
using LiveLearn.Enrollment.Domain.Errors;
using LiveLearn.Enrollment.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Enrollment.Infrastructure.Queries.Handlers;

internal sealed class GetMyEnrollmentStatusQueryHandler(ICurrentUser currentUser, ReadDbContext dbContext) : IQueryHandler<GetMyEnrollmentStatusQuery, EnrollmentStatusDto>
{
    public async Task<Result<EnrollmentStatusDto>> Handle(GetMyEnrollmentStatusQuery request, CancellationToken ct)
    {
        var status = await dbContext.Enrollments
            .Where(e => e.StudentId == currentUser.UserId && e.CourseId == request.CourseId)
            .Select(e => new EnrollmentStatusDto(e.Id, e.Status))
            .FirstOrDefaultAsync(ct);

        if (status is null) return Result<EnrollmentStatusDto>.Failure(EnrollmentErrors.NotFound);
        return status;
    }
}
