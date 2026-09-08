using LiveLearn.Assessment.Application.Dto;
using LiveLearn.Assessment.Application.Queries;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.Assessment.Infrastructure.Contexts;
using LiveLearn.BuildingBlocks;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.QueryHandlers;


internal sealed class GetQuizByIdQueryHandler(ReadDbContext dbContext) : IQueryHandler<GetQuizByIdQuery, QuizDto>
{
    public async Task<Result<QuizDto>> Handle(GetQuizByIdQuery request, CancellationToken ct)
    {
        var quiz = await dbContext.Quizzes.FirstOrDefaultAsync(e => e.Id == request.Id, ct);
        if (quiz is null) return Result<QuizDto>.Failure(TaskErrors.NotFound);

        return new QuizDto(
            quiz.Id,
            quiz.Title,
            quiz.Questions
                .Select(q => new QuestionDto(
                    q.Id,
                    q.Text,
                    q.Answers
                        .Select(a => new AnswerDto(a.Id, a.Content)).ToList(),
                    request.IncludeCorrectAnswer ? q.CorrectAnswerId : null
                    ))
                .ToList());

    }
}
