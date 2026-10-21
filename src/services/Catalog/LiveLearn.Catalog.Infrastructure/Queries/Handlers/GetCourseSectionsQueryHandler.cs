using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Queries;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Domain.Enums;
using LiveLearn.Catalog.Domain.Errors;
using LiveLearn.Catalog.Infrastructure.Contexts;
using LiveLearn.Catalog.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Queries.Handlers;

internal sealed class GetCourseSectionsQueryHandler(ICourseAccess courseAccess, ReadDbContext dbContext) : IQueryHandler<GetCourseSectionsQuery, IReadOnlyList<SectionDto>>
{
    public async Task<Result<IReadOnlyList<SectionDto>>> Handle(GetCourseSectionsQuery request, CancellationToken ct)
    {
        //this query assume each user (even unauthenticated) can see the course sections
        //Course owner and admin can read sections regardless of course status including archived content.
        // Other users see only if course is published AND only current revision sections.
        var access = await courseAccess.GetBrowseAccessAsync(request.CourseId, ct);
        if (access is null || access.Mode == CourseAccessMode.None)
            return Result<IReadOnlyList<SectionDto>>.Failure(CourseErrors.NotFound);

        var sections = await dbContext.Sections
            .Where(s => s.CourseId == request.CourseId)
            .Where(SectionReadModel.VisibleTo(access))
            .OrderBy(s => s.Order)
            .Select(CourseProjections.ToSection(access))
            .AsSplitQuery()
            .ToListAsync(ct);

        return sections;
    }
}
