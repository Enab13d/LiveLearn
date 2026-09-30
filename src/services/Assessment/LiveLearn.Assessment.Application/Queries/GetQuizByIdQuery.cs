using LiveLearn.Assessment.Application.Dto;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Queries;


public sealed record GetQuizByIdQuery(Guid Id, Guid RequesterId) : IQuery<QuizDto>;
