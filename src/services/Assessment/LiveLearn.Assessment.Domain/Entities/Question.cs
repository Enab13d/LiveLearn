using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Domain.Entities;

public sealed class Question : Entity<Guid>
{
    private Question() { }

    internal Question(string text, List<string> answerContents, int correctAnswerIdx)
    {
        Id = Guid.NewGuid();
        Text = text;
        var createdAnswers = answerContents.Select(content => new Answer(Guid.NewGuid(), content)).ToList();
        CorrectAnswerId = createdAnswers[correctAnswerIdx].Id;
        foreach (var answer in createdAnswers)
        {
            _answers.Add(answer);
        }
    }

    public string Text { get; private set; } = string.Empty;

    private readonly List<Answer> _answers = [];
    public IReadOnlyCollection<Answer> Answers => _answers.AsReadOnly();
    public Guid CorrectAnswerId { get; private set; }

}
