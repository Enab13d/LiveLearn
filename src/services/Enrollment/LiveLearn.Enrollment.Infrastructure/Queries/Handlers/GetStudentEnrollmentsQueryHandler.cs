using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Application.Dto;
using LiveLearn.Enrollment.Application.Queries;
using LiveLearn.Enrollment.Application.Services;
using LiveLearn.Enrollment.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Enrollment.Infrastructure.Queries.Handlers;

internal sealed class GetStudentEnrollmentsQueryHandler(ICurrentUser currentUser, ReadDbContext dbContext) : IQueryHandler<GetStudentEnrollmentsQuery, PagedResult<EnrollmentDto>>
{
    public async Task<Result<PagedResult<EnrollmentDto>>> Handle(GetStudentEnrollmentsQuery request, CancellationToken ct)
    {
        var query = dbContext.Enrollments.Where(e => e.StudentId == currentUser.UserId);

        int totalCount = await query.CountAsync(ct);

        var enrollments = await query
            .OrderByDescending(e => e.EnrolledAt)
            .ThenBy(e => e.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new EnrollmentDto(
                e.Id,
                e.CourseId,
                e.Status,
                e.CourseRevision,
                e.EnrolledAt,
                e.CompletedAt,
                e.Items.Count(i => i.CompletedAt != null),
                e.Items.Count))
            .ToListAsync(ct);

        return new PagedResult<EnrollmentDto>(enrollments, totalCount, request.PageNumber, request.PageSize);
    }
}
