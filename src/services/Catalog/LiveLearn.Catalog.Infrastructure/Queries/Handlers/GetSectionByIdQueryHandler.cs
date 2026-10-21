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

internal sealed class GetSectionByIdQueryHandler(ICourseAccess courseAccess, ReadDbContext dbContext) : IQueryHandler<GetSectionByIdQuery, SectionDto>
{
    public async Task<Result<SectionDto>> Handle(GetSectionByIdQuery request, CancellationToken ct)
    {
        //this query assume each user (even unauthenticated) can see the section outline
        //Course owner and admin can read section regardless of it's status including archived content.
        // Other users see only if section is published AND only current revision content.
        var access = await courseAccess.GetBrowseAccessAsync(request.CourseId, ct);
        if (access is null || access.Mode == CourseAccessMode.None)
            return Result<SectionDto>.Failure(CourseErrors.NotFound);

        var section = await dbContext.Sections
            .Where(s => s.Id == request.SectionId && s.CourseId == request.CourseId)
            .Where(SectionReadModel.VisibleTo(access))
            .Select(CourseProjections.ToSection(access))
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct);

        if (section is null) return Result<SectionDto>.Failure(CourseErrors.SectionNotFound);
        return section;
    }
}
