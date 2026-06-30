using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.Entities;

public sealed class Lecture : Entity<Guid>
{
    private Lecture() { }

    public Guid SectionId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public LectureType Type { get; private set; }

    public int Order { get; private set; }

    public TimeSpan Duration { get; private set; }



}





