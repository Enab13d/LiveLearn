using LiveLearn.Assessment.Application.Dto;
using LiveLearn.Assessment.Application.Queries;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.Assessment.Infrastructure.Contexts;
using LiveLearn.BuildingBlocks;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.QueryHandlers;


internal sealed class GetHomeworkByIdQueryHandler(ReadDbContext dbContext) : IQueryHandler<GetHomeworkByIdQuery, HomeworkDto>
{
    public async Task<Result<HomeworkDto>> Handle(GetHomeworkByIdQuery request, CancellationToken ct)
    {
        var homework = await dbContext.Homeworks.FirstOrDefaultAsync(e => e.Id == request.Id, ct);
        if (homework is null) return Result<HomeworkDto>.Failure(TaskErrors.NotFound);
        return new HomeworkDto(homework.Id, homework.TutorId, homework.Title, homework.Description);

    }
}
