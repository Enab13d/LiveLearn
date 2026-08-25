using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Queries;
using LiveLearn.Catalog.Domain.Errors;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Queries.Handlers;

internal sealed class GetCourseSectionsQueryHandler(ReadDbContext dbContext) : IQueryHandler<GetCourseSectionsQuery, IReadOnlyList<SectionDto>>
{
    public async Task<Result<IReadOnlyList<SectionDto>>> Handle(GetCourseSectionsQuery request, CancellationToken ct)
    {
        var courseExists = await dbContext.Courses.AnyAsync(e => e.Id == request.CourseId, ct);
        if (!courseExists) return Result<IReadOnlyList<SectionDto>>.Failure(CourseErrors.NotFound);

        var sections = await dbContext.Sections
            .Where(e => e.CourseId == request.CourseId)
            .OrderBy(e => e.Order)
            .Select(e => new SectionDto(
                e.Id,
                e.Title,
                e.Order,
                e.Lectures
                    .Select(l => new LectureDto(l.Id, l.Title, l.Type, l.Order, l.DurationInSeconds))
                    .ToList(),
                e.SectionTasks
                    .Select(e => new SectionTaskDto(e.SectionId, e.CourseId, e.TaskId, e.TaskType))
                    .ToList()
                ))
            .AsSplitQuery()
            .ToListAsync(ct);

        return sections;
    }
}
