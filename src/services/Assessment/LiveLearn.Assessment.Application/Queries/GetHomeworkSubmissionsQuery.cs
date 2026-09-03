using LiveLearn.Assessment.Application.Dto;
using LiveLearn.Assessment.Domain.Enums;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Queries;


public sealed record GetHomeworkSubmissionsQuery(
    Guid HomeworkId,
    Guid TutorId,
    int PageNumber = 1,
    int PageSize = 10,
    Guid? StudentId = null,
    Guid? CourseId = null,
    Guid? SectionId = null,
    HomeworkStatus? Status = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IQuery<PagedResult<HomeworkSubmissionDto>>;
