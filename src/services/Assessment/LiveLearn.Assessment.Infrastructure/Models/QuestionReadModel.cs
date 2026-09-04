namespace LiveLearn.Assessment.Infrastructure.Models;


internal sealed class QuestionReadModel
{
    public Guid Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public List<AnswerReadModel> Answers { get; set; } = [];

    public Guid CorrectAnswerId { get; set; }
}
