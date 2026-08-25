using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Queries;
using LiveLearn.Catalog.Domain.Errors;
using LiveLearn.Catalog.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Queries.Handlers;

internal sealed class GetSectionByIdQueryHandler(ReadDbContext dbContext) : IQueryHandler<GetSectionByIdQuery, SectionDto>
{
    public async Task<Result<SectionDto>> Handle(GetSectionByIdQuery request, CancellationToken ct)
    {
        var section = await dbContext.Sections
            .Where(e => e.Id == request.SectionId && e.CourseId == request.CourseId)
            .Select(e => new SectionDto(
                e.Id,
                e.Title,
                e.Order,
                e.Lectures
                    .Select(l => new LectureDto(l.Id, l.Title, l.Type, l.Order, l.DurationInSeconds))
                    .ToList(),
                e.SectionTasks
                    .Select(e => new SectionTaskDto(
                        e.SectionId, e.CourseId, e.TaskId, e.TaskType))
                    .ToList()

                )
                )
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct);

        if (section is null) return Result<SectionDto>.Failure(CourseErrors.SectionNotFound);
        return section;
    }
}
