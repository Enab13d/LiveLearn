using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.Enums;

namespace LiveLearn.Catalog.Domain.Entities;


public sealed class Section : Entity<Guid>
{
    private Section() { }

    public Guid CourseId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public int Order { get; private set; }
    private readonly List<Lecture> _lectures = [];

    public IReadOnlyCollection<Lecture> Lectures => _lectures.AsReadOnly();

    internal Lecture AddLecture(Guid lectureId, string title, LectureType lectureType, int order, TimeSpan duration)
    {
        var lecture = new Lecture(lectureId, Id, title, lectureType, order, duration);
        _lectures.Add(lecture);
        return lecture;
    }

}
