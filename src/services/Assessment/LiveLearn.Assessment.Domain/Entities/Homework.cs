using LiveLearn.Assessment.Domain.DomainEvents;

namespace LiveLearn.Assessment.Domain.Entities;


public sealed class Homework : AssessmentTask
{

    private Homework() { }
    public string Description { get; private set; } = string.Empty;

    public static Homework Create(Guid id, string title, Guid tutorId, string description)
    {
        Homework homework = new()
        {
            Id = id,
            Title = title,
            TutorId = tutorId,
            Description = description
        };

        homework.RaiseDomainEvent(new TaskCreatedDomainEvent(id, tutorId, nameof(Homework)));
        return homework;

    }

    public void UpdateTitle(string title)
    {
        Title = title;
    }
    public void UpdateDescription(string description)
    {
        Description = description;
    }


}
