namespace LiveLearn.Assessment.Infrastructure.Models;


internal sealed class HomeworkReadModel
{
    public Guid Id { get; set; }

    public Guid TutorId { get; set; }
    public Guid SectionId { get; set; }
    public Guid CourseId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
