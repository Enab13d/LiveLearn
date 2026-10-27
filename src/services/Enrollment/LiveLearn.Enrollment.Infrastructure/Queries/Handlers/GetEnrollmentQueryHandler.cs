using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Application.Dto;
using LiveLearn.Enrollment.Application.Queries;
using LiveLearn.Enrollment.Application.Services;
using LiveLearn.Enrollment.Domain.Errors;
using LiveLearn.Enrollment.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Enrollment.Infrastructure.Queries.Handlers;

internal sealed class GetEnrollmentQueryHandler(ICurrentUser currentUser, ReadDbContext dbContext) : IQueryHandler<GetEnrollmentQuery, EnrollmentDetailDto>
{
    public async Task<Result<EnrollmentDetailDto>> Handle(GetEnrollmentQuery request, CancellationToken ct)
    {
        var enrollment = await dbContext.Enrollments
            .Where(e => e.Id == request.EnrollmentId && e.StudentId == currentUser.UserId)
            .Select(e => new EnrollmentDetailDto(
                e.Id,
                e.CourseId,
                e.Status,
                e.CourseRevision,
                e.EnrolledAt,
                e.CompletedAt,
                e.Items.Count(i => i.CompletedAt != null),
                e.Items.Count,
                e.Items.Select(i => new EnrollmentItemDto(i.ItemId, i.Kind, i.CompletedAt)).ToList()))
            .FirstOrDefaultAsync(ct);

        if (enrollment is null) return Result<EnrollmentDetailDto>.Failure(EnrollmentErrors.NotFound);
        return enrollment;
    }
}
