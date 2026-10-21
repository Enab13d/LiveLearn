using LiveLearn.Assessment.Application.Dto;
using LiveLearn.Assessment.Application.Queries;
using LiveLearn.Assessment.Application.Services;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.Assessment.Infrastructure.Contexts;
using LiveLearn.BuildingBlocks;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.QueryHandlers;


internal sealed class GetQuizByIdQueryHandler(
    ReadDbContext dbContext,
    IEnrollmentLookup enrollmentLookup,
    ICourseTaskLookup courseTaskLookup
) : IQueryHandler<GetQuizByIdQuery, QuizDto>
{
    public async Task<Result<QuizDto>> Handle(GetQuizByIdQuery request, CancellationToken ct)
    {
        var quiz = await dbContext.Quizzes.FirstOrDefaultAsync(e => e.Id == request.Id, ct);
        if (quiz is null) return Result<QuizDto>.Failure(TaskErrors.NotFound);

        // owner → full DTO; otherwise enrolled in a course the quiz was ever assigned to → student DTO
        bool isOwner = request.RequesterId == quiz.TutorId;
        if (!isOwner
            && (request.CourseId is not Guid courseId
                || !await enrollmentLookup.IsEnrolledAsync(request.RequesterId, courseId, ct)
                || !await courseTaskLookup.WasEverAssignedAsync(courseId, quiz.Id, ct)))
            return Result<QuizDto>.Failure(TaskErrors.Forbidden);

        return new QuizDto(
            quiz.Id,
            quiz.Title,
            quiz.Questions
                .Select(q => new QuestionDto(
                    q.Id,
                    q.Text,
                    q.Answers
                        .Select(a => new AnswerDto(a.Id, a.Content)).ToList(),
                    isOwner ? q.CorrectAnswerId : null
                    ))
                .ToList());

    }
}
