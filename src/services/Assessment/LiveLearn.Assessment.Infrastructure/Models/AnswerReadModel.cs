namespace LiveLearn.Assessment.Infrastructure.Models;

internal sealed class AnswerReadModel
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
}
