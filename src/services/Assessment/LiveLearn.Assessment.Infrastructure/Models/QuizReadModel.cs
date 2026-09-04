namespace LiveLearn.Assessment.Infrastructure.Models;


internal sealed class QuizReadModel
{
    public Guid Id { get; set; }

    public Guid TutorId { get; set; }
    public Guid SectionId { get; set; }
    public Guid CourseId { get; set; }

    public string Title { get; set; } = string.Empty;
    public int PassingScore { get; set; }

    public List<QuestionReadModel> Questions { get; set; } = [];
}
