using LiveLearn.Assessment.Domain.Enums;

namespace LiveLearn.Assessment.API.Dto.QueryParameters;


public sealed class HomeworkQueryParameters : PageableQueryParams
{
    public Guid? StudentId { get; init; }
    public Guid? CourseId { get; init; }
    public Guid? SectionId { get; init; }
    public HomeworkStatus? Status { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}
