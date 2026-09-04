namespace LiveLearn.Assessment.Infrastructure.Models;

internal sealed class AssessmentTaskReadModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid TutorId { get; set; }
    public Guid SectionId { get; set; }
    public Guid CourseId { get; set; }
    public string TaskType { get; set; } = string.Empty;
}
