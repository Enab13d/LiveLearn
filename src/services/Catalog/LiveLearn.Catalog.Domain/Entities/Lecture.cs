using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.Enums;

namespace LiveLearn.Catalog.Domain.Entities;

public sealed class Lecture : Entity<Guid>
{
    private Lecture() { }
    internal Lecture(Guid id, Guid sectionId, string title, LectureType type, int order, TimeSpan duration)
    {
        Id = id;
        SectionId = sectionId;
        Title = title;
        Type = type;
        Order = order;
        Duration = duration;
    }

    public Guid SectionId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public LectureType Type { get; private set; }

    public int Order { get; private set; }

    public TimeSpan Duration { get; private set; }

    internal void SetOrder(int order)
    {
        Order = order;
    }


}





