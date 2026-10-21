using LiveLearn.Assessment.Application.Dto;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Queries;


public sealed record GetHomeworkByIdQuery(Guid Id, Guid RequesterId, Guid? CourseId) : IQuery<HomeworkDto>;
