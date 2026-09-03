using LiveLearn.Assessment.Application.Dto;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Queries;


public sealed record GetQuizAttemptsQuery(Guid StudentId, Guid QuizId, int PageNumber = 1, int PageSize = 10) : IQuery<PagedResult<QuizAttemptDto>>;
