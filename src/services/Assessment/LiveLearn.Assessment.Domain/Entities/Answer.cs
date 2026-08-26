using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Domain.Entities;

public sealed class Answer : Entity<Guid>
{
    private Answer() { }
    public string Content { get; private set; } = string.Empty;

    internal Answer(Guid id, string content)
    {
        Id = id;
        Content = content;
    }
}
