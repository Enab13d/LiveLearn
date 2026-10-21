using LiveLearn.Assessment.Application.Dto;
using LiveLearn.Assessment.Application.Queries;
using LiveLearn.Assessment.Application.Services;
using LiveLearn.Assessment.Infrastructure.Contexts;
using LiveLearn.BuildingBlocks;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.QueryHandlers;


internal sealed class GetQuizAttemptsQueryHandler(ICurrentUser currentUser, ReadDbContext dbContext) : IQueryHandler<GetQuizAttemptsQuery, PagedResult<QuizAttemptDto>>
{
    public async Task<Result<PagedResult<QuizAttemptDto>>> Handle(GetQuizAttemptsQuery request, CancellationToken ct)
    {
        var query = dbContext.QuizAttempts
            .AsQueryable()
            .Where(e => e.StudentId == currentUser.UserId && e.QuizId == request.QuizId);

        int totalCount = await query.CountAsync(ct);

        var quizAttempts = await query
            .OrderByDescending(e => e.AttemptedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new QuizAttemptDto(e.Id, e.CourseId, e.Score, e.IsPassed, e.AttemptedAt))
            .ToListAsync(ct);

        return new PagedResult<QuizAttemptDto>(quizAttempts, totalCount, request.PageNumber, request.PageSize);

    }
}
