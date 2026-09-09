namespace LiveLearn.Assessment.Infrastructure.Models;


internal sealed class QuizReadModel : AssessmentTaskReadModel
{
    public int PassingScore { get; set; }

    public List<QuestionReadModel> Questions { get; set; } = [];
}
