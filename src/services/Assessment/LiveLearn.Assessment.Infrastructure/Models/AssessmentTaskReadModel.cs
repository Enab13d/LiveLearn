namespace LiveLearn.Assessment.Infrastructure.Models;


internal class AssessmentTaskReadModel
{

    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid TutorId { get; set; }
    public DateTimeOffset? ArchivedAt { get; set; }

    public string TaskType { get; set; } = string.Empty;
}
